using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.Events;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Domain.Entities;

public class VolunteerRequest : DomainEntity<VolunteerRequestId>
{
    public AdminId? AdminId { get; private set; }

    public UserId UserId { get; private set; } = null!;

    public VolunteerInfo VolunteerInfo { get; private set; } = null!;

    public VolunteerRequestStatus Status { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public RevisionComment? RevisionComment { get; private set; }

    public DateTime? RejectedAt { get; private set; }

    public VolunteerRequest(VolunteerRequestId id, UserId userId, VolunteerInfo volunteerInfo)
        : base(id)
    {
        UserId = userId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Submitted).Value;
        VolunteerInfo = volunteerInfo;
    }

    // ef core
    // ReSharper disable once UnusedMember.Local
    private VolunteerRequest(VolunteerRequestId id)
        : base(id)
    {
    }

    public UnitResult<Error> SetSubmitted(VolunteerInfo volunteerInfo)
    {
        // при создании заявки (через конструктор) её статус по умолчанию всегда submitted
        // а вот установить этот статус методом можно только тогда, когда её текущий статус - RevisionRequired
        if (Status.Value != VolunteerRequestStatusEnum.RevisionRequired)
        {
            return Errors.General.Conflict("new VolunteerRequestStatus is inaccessible from the current one");
        }

        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Submitted).Value;
        VolunteerInfo = volunteerInfo;

        // добавляем доменное событие - заявка была исправлена
        if (RevisionComment is not null)
            AddDomainEvent(new VolunteerRequestWasAmendedEvent(UserId, AdminId!.Value, Id));

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetOnReview(AdminId adminId)
    {
        // OnReview можно установить только тогда, когда текущий статус заявки - Submitted
        if (Status.Value != VolunteerRequestStatusEnum.Submitted)
        {
            return Errors.General.Conflict("new VolunteerRequestStatus is inaccessible from the current one");
        }

        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.OnReview).Value;

        // добавляем доменное событие - заявка взята на рассмотрение
        AddDomainEvent(new VolunteerRequestWasTakenOnReviewEvent(UserId, AdminId, Id));

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetRevisionRequired(AdminId adminId, RevisionComment revisionComment)
    {
        // RevisionRequired можно установить только тогда, когда текущий статус заявки - OnReview
        if (Status.Value != VolunteerRequestStatusEnum.OnReview)
        {
            return Errors.General.Conflict("new VolunteerRequestStatus is inaccessible from the current one");
        }

        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.RevisionRequired).Value;
        RevisionComment = revisionComment;

        // добавляем доменное событие - заявка отправлена на доработку
        AddDomainEvent(new VolunteerRequestRequiredRevisionEvent(UserId, adminId, Id));

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetRejected(AdminId adminId)
    {
        // Rejected можно установить только тогда, когда текущий статус заявки - OnReview
        if (Status.Value != VolunteerRequestStatusEnum.OnReview)
        {
            return Errors.General.Conflict("new VolunteerRequestStatus is inaccessible from the current one");
        }

        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Rejected).Value;
        RejectedAt = DateTime.UtcNow;

        // добавляем доменное событие - заявка отклонена
        AddDomainEvent(new VolunteerRequestWasRejectedEvent(UserId, AdminId, Id));

        return UnitResult.Success<Error>();
    }

    public UnitResult<Error> SetApproved(AdminId adminId)
    {
        // Approved можно установить только тогда, когда текущий статус заявки - OnReview
        if (Status.Value != VolunteerRequestStatusEnum.OnReview)
        {
            return Errors.General.Conflict("new VolunteerRequestStatus is inaccessible from the current one");
        }

        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Approved).Value;

        // добавляем доменное событие - заявка одобрена
        AddDomainEvent(new VolunteerRequestWasApprovedEvent(UserId, adminId, Id));

        return UnitResult.Success<Error>();
    }
}
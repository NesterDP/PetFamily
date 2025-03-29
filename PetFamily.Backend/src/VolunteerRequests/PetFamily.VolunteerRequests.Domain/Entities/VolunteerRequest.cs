using CSharpFunctionalExtensions;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects.Ids;
using PetFamily.VolunteerRequests.Domain.ValueObjects;

namespace PetFamily.VolunteerRequests.Domain.Entities;

public class VolunteerRequest
{
    public VolunteerRequestId Id { get; private set; }
    public AdminId? AdminId { get; private set; }
    public UserId UserId { get; private set; }
    public VolunteerInfo VolunteerInfo { get; private set; }
    public VolunteerRequestStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public RevisionComment? RevisionComment { get; private set; }
    
    public DateTime? RejectedAt { get; private set; }
    
    private VolunteerRequest() { } // ef core

    public VolunteerRequest(UserId userId, VolunteerInfo volunteerInfo)
    {
        Id = VolunteerRequestId.NewVolunteerRequestId();
        UserId = userId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Submitted).Value;
        VolunteerInfo = volunteerInfo;
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

        return UnitResult.Success<Error>();
    }
}
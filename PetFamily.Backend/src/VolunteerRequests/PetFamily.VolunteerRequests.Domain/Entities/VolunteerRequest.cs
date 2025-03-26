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
    public RejectionComment? RejectionComment { get; private set; }

    private VolunteerRequest() { } // ef core
    
    public VolunteerRequest(UserId userId, VolunteerInfo volunteerInfo)
    {
        Id = VolunteerRequestId.NewVolunteerRequestId();
        UserId = userId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Submitted).Value;
        VolunteerInfo = volunteerInfo;
    }
    
    public void SetSubmitted(AdminId adminId)
    {
        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Submitted).Value;
    }

    public void SetOnReview(AdminId adminId)
    {
        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.OnReview).Value;
    }
    
    public void SetRevisionRequired(AdminId adminId, RejectionComment rejectionComment)
    {
        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.RevisionRequired).Value;
        RejectionComment = rejectionComment;
    }
    
    public void SetRejected(AdminId adminId)
    {
        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Rejected).Value;
    }
    
    public void SetApproved(AdminId adminId)
    {
        AdminId = adminId;
        Status = VolunteerRequestStatus.Create(VolunteerRequestStatusEnum.Approved).Value;
    }
}
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Queries.GetUserById;
using PetFamily.Accounts.Contracts.Dto;
using PetFamily.Core.Abstractions;
using PetFamily.IntegrationTests.Accounts.Heritage;
using PetFamily.IntegrationTests.General;
using PetFamily.SharedKernel.Constants;
using PetFamily.SharedKernel.CustomErrors;
using PetFamily.SharedKernel.ValueObjects;

namespace PetFamily.IntegrationTests.Accounts.HandlersTests;

public class GetUserInfoByIdTests : AccountsTestsBase
{
    private readonly IQueryHandler<Result<UserInfoDto, ErrorList>, GetUserInfoByIdQuery> _sut;

    public GetUserInfoByIdTests(AccountsTestsWebFactory factory)
        : base(factory)
    {
        _sut = Scope.ServiceProvider
            .GetRequiredService<IQueryHandler<Result<UserInfoDto, ErrorList>, GetUserInfoByIdQuery>>();
    }

    [Fact]
    public async Task GetUserInfoById_success_should_return_user_info_with_correct_fields()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";
        var avatarId = Guid.NewGuid();

        var user = await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);

        user.Avatar = Avatar.Create(avatarId, DomainConstants.PNG).Value;
        await AccountsDbContext.SaveChangesAsync();

        var query = new GetUserInfoByIdQuery(user.Id);

        Factory.SetupSuccessGetFilesPresignedUrlsMock([avatarId]);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Email.Should().Be(EMAIL);
        result.Value.ParticipantAccount.Should().NotBeNull();
        result.Value.VolunteerAccount.Should().BeNull();
        result.Value.AdminAccount.Should().BeNull();
        result.Value.Avatar.Id.Should().Be(avatarId);
        result.Value.Avatar.Url.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserInfoById_failure_should_should_return_error_because_of_no_user_with_such_id()
    {
        // arrange
        const string EMAIL = "test@mail.com";
        const string USERNAME = "testUserName";
        const string PASSWORD = "Password121314s.";

        await DataGenerator.SeedUserAsync(
            USERNAME,
            EMAIL,
            PASSWORD,
            UserManager,
            RoleManager,
            AccountManager);

        var query = new GetUserInfoByIdQuery(Guid.NewGuid());

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserInfoById_success_admin_should_have_only_admin_account()
    {
        // arrange
        const string EMAIL = "admin@admin.com";

        var user = await UserManager.FindByEmailAsync(EMAIL);

        var query = new GetUserInfoByIdQuery(user!.Id);

        // act
        var result = await _sut.HandleAsync(query, CancellationToken.None);

        // assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Email.Should().Be(EMAIL);
        result.Value.ParticipantAccount.Should().BeNull();
        result.Value.VolunteerAccount.Should().BeNull();
        result.Value.AdminAccount.Should().NotBeNull();
    }
}
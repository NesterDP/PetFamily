using System.Reflection;

namespace PetFamily.VolunteerRequests.Contracts.StaticClasses;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
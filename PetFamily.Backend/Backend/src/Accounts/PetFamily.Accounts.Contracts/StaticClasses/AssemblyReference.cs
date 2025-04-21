using System.Reflection;

namespace PetFamily.Accounts.Contracts.StaticClasses;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}
using UnrealBuildTool;

public class KTween : ModuleRules
{
    public KTween(ReadOnlyTargetRules Target) : base(Target)
    {
        PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;

        PublicDependencyModuleNames.AddRange(new string[] {
            "Core", "CoreUObject", "Engine", "InputCore", "UMG", "NetCore"
        });
    }
}

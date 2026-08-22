Get-ChildItem -Recurse -Filter *.csproj | ForEach-Object {
    $project = $_
    [xml]$xml = Get-Content $project.FullName

    foreach ($ref in $xml.Project.ItemGroup.ProjectReference) {
        if ($ref.Include) {
            $target = Split-Path $ref.Include -Leaf
            Write-Output "$($project.BaseName)  -->  $([System.IO.Path]::GetFileNameWithoutExtension($target))"
        }
    }
}

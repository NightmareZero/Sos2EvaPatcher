# 记录当前目录
$currentDir = $PWD.Path

# 创建一个空的文本文件来存储远程URL
$dependenceFile = New-Item -Path . -Name "dependence.txt" -ItemType "file" -Force

# 获取当前目录的上级目录下的所有子目录，排除当前目录
$directories = Get-ChildItem -Directory -Path ".." | Where-Object { $_.FullName -ne $PWD.Path }

# 遍历每个子目录
foreach ($dir in $directories) {
    # 检查子目录是否包含.git目录
    if (Test-Path -Path "$dir/.git") {
        # 进入该目录
        Set-Location -Path $dir
        # 获取远程URL
        $remoteUrl = git config --get remote.origin.url
        # 获取目录名
        $dirName = $dir.Name
        Write-Host "${dirName}: ${remoteUrl}"
        # 将远程URL写入文件
        Add-Content -Path $dependenceFile.FullName -Value "${dirName}: ${remoteUrl}"
    }
}

# 返回到原始目录
Set-Location -Path $currentDir
if (Test-Path "nuget.exe") {
	Invoke-Expression "./nuget restore"
}
elseif((Get-Command "nuget" -ErrorAction SilentlyContinue) -ne $null) {
	nuget restore
}
else {
	"Nuget was not found and is required to run bootstrap.ps. Download and retry now?"
	choice /c yn
	if ($LASTEXITCODE -eq 1) {
		"Downloading nuget..."
		$nugetLocation = "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
		(New-Object Net.WebClient).DownloadFile($nugetLocation, "$PSScriptRoot\nuget.exe")
		Invoke-Expression "./nuget restore"
	}
}

try {
	git --version
}
catch {
	"Git was not found and is required to run bootstrap.bat. Download git from https://git-scm.com/download and during installation choose `"Use Git from the Windows Command Prompt`"."
}

git clone "https://github.com/HearthSim/Hearthstone-Deck-Tracker.git" "Hearthstone-Deck-Tracker"

cd Hearthstone-Deck-Tracker

./bootstrap.ps1
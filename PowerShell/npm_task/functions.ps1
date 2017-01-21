function Distribute($private:func_list, $private:param_list) {
	foreach ($f in $private:func_list) {
		switch ($f) {
			'clean' { 
				$private:param_list | %{ clean $_ }
			}
			'dist' {
				$private:param_list | %{ dist (path-module $_.module) $_ }
			}
		}
	}
}

function clean($private:param) {
	$path_output = (path-output $private:param.output)
	@($private:param.directory, $private:param.file) | %{
		$_ | ? { $_ -ne $null } | %{
			($tgt = [IO.Path]::Combine($path_output, $_.dest)) |oh
			if (Test-Path $tgt) {
				Remove-Item $tgt -Recurse
			}
		}
	}
}
function dist([string]$private:path, $private:param) {
	dist-directory $private:path $private:param
	dist-file $private:path $private:param
	[IO.Directory]::EnumerateDirectories($private:path) | %{
		dist $_ $private:param
	}
}
function dist-directory([string]$private:path, $private:param) {
	if ($private:param.directory -eq $null) { return }
	$layout = $private:param.directory
	$path_output = (path-output $private:param.output)
	$path_module = (path-module $private:param.module)
	$path_rel = $private:path.Remove(0, $path_module.Length)

	$layout | ?{ $path_rel -eq $_.where } | %{
		$dst = [IO.Path]::Combine($path_output, $_.dest)
		($src = [IO.Path]::Combine($private:path, "*"))|oh
		New-Item "$dst" -type directory -force |Out-Null
		if ($_.recurse) {
			Copy-Item $src "$dst" -Recurse
		} else {
			Copy-Item $src "$dst"
		}
	}
}
function dist-file([string] $private:path, $private:param) {
	if ($private:param.file -eq $null) { return }
	$layout = $private:param.file
	$path_output = (path-output $private:param.output)
	$path_module = (path-module $private:param.module)
	$path_rel = $private:path.Remove(0, $path_module.Length)

	$layout | ?{ $path_rel -eq $_.where } | %{
		$dst = [IO.Path]::Combine($path_output, $_.dest)
		($src = [IO.Path]::Combine($private:path, $_.search))|oh
		New-Item "$dst" -type directory -force |Out-Null
		Copy-Item $src "$dst"
	}
}
function path-module([string] $private:name) {
	[IO.Path]::Combine((pwd).Path, "node_modules", $private:name)
}
function path-output([string] $private:output) {
	[IO.Path]::Combine((pwd).Path, $private:output)
}

param([string[]]$func_list)
$path_cd = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$path_cd\functions.ps1"

#ファイル配置レイアウト定義
Distribute $func_list @(
	@{
		module = "bootstrap";
		output = "..\..\applications\Sandbox.Web";
		directory = @(
			@{ where = "\dist\css";   recurse = $false; dest = "Content\bootstrap\css" }
			@{ where = "\dist\fonts"; recurse = $false; dest = "Content\bootstrap\fonts" }
			@{ where = "\dist\js";    recurse = $false; dest = "Scripts\bootstrap\" }
		);
	}
	@{
		module = "jquery";
		output = "..\..\applications\Sandbox.Web";
		directory = @(
			@{ where = "\dist"; recurse = $false; dest = "Scripts\jquery" }
		);
	}
	@{
		module = "jquery-ui-1-11-4";
		output = "..\..\applications\Sandbox.Web";
		directory = @(
			@{ where = "\images"; recurse = $false; dest = "Content\jquery-ui\images" }
		);
		file = @(
			@{ where = ""; search = "*.css"; dest = "Content\jquery-ui" }
			@{ where = "";  search = "*.js"; dest = "Scripts\jquery-ui" }
		);
	}
	@{
		module = "jquery-ui-1-11-1";
		output = "..\..\applications\Sandbox.Web";
		file = @(
			@{ where = "\ui";                  search = "*.js"; dest = "Scripts\jquery-ui\widgets" }
			@{ where = "\ui\i18n"; search = "datepicker-ja.js"; dest = "Scripts\jquery-ui\widgets" }
		);
	}
	@{
		module = "vue";
		output = "..\..\applications\Sandbox.Web";
		file = @(
			@{ where = "\dist"; search = "*.js"; dest = "Scripts\vue" }
		);
	}
	@{
		module = "slickgrid";
		output = "..\..\applications\Sandbox.Web";
		directory = @(
			@{ where = "\images"; recurse = $false; dest = "Content\slickgrid\images" }
		);
		file = @(
			@{ where = "";               search = "*.css"; dest = "Content\slickgrid" }
			@{ where = "";                search = "*.js"; dest = "Scripts\slickgrid"}
			@{ where = "\controls";      search = "*.css"; dest = "Content\slickgrid\controls" }
			@{ where = "\controls";       search = "*.js"; dest = "Scripts\slickgrid\controls"}
			@{ where = "\plugins";       search = "*.css"; dest = "Content\slickgrid\plugins" }
			@{ where = "\plugins";        search = "*.js"; dest = "Scripts\slickgrid\plugins"}
			@{ where = "\lib";           search = "*.css"; dest = "Content\slickgrid\lib" }
			@{ where = "\lib";            search = "*.js"; dest = "Scripts\slickgrid\lib"}
			@{ where = "\example"; search = "example.css"; dest = "Content\slickgrid\example"}
			@{ where = "\example";        search = "*.js"; dest = "Scripts\slickgrid\example"}
		);
	}
	<# 定義サンプル
	@{
		output = "このプロジェクトフォルダをカレントとした、出力先相対パス";
		module = "対象のモジュール名（node_modules以下のフォルダ名）";
		directory = @(
			配列。whereと一致するフォルダを、destへコピーします。recurseがtrueのときサブフォルダも対象とします。
			@{ where = "\モジュールフォルダ配下のパス"; recurse = $true|$false; dest = "outputをカレントとしたコピー先相対パス" }
		);
		file = @(
			配列。whereと一致するフォルダ配下の、searchと一致するファイルを、destへコピーします。
			@{ where = "\モジュールフォルダ配下のパス"; search = "コピー対象。ワイルドカード指定可"; dest = "outputをカレントとしたコピー先相対パス" }
		);
	}
	#>
)

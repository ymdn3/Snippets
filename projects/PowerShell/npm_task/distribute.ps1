param([string[]]$func_list)
$private:path_cd = Split-Path -Parent $MyInvocation.MyCommand.Path
. "$private:path_cd\functions.ps1"

#ファイル配置レイアウト定義

$local:output = "..\Sandbox.Web"

Distribute $func_list @(
	@{#bootstrap(Umi)
		module = "bower_components\Umi";
		output = $local:output;
		file = @(
			@{ where = "";      search = "README.md"; dest = "Content\bootstrap"}
			@{ where = "\dist\css"; search = "*.css"; dest = "Content\bootstrap\css" }
			@{ where = "\dist\fonts"; search = "*.*"; dest = "Content\bootstrap\fonts" }
			@{ where = "\dist\js";   search = "*.js"; dest = "Scripts\bootstrap"}
		);
	}
	@{#jquery
		module = "node_modules\jquery";
		output = $local:output;
		file = @(
			@{ where = "\dist";  search = "*.js"; dest = "Scripts\jquery" }
			@{ where = "\dist"; search = "*.map"; dest = "Scripts\jquery" }
		);
	}
	@{#jquery-ajax-unobtrusive
		module = "node_modules\jquery-ajax-unobtrusive";
		output = $local:output;
		file = @(
			@{ where = "";  search = "jquery.*.js"; dest = "Scripts\jquery" }
		);
	}
	@{#jquery-validation
		module = "node_modules\jquery-validation";
		output = $local:output;
		file = @(
			@{ where = "\dist";                        search = "*.js"; dest = "Scripts\jquery-validation"}
			@{ where = "\dist\localization"; search = "messages_ja.js"; dest = "Scripts\jquery-validation"}
		);
	}
	@{#jquery-ui
		module = "node_modules\jquery-ui-dist";
		output = $local:output;
		directory = @(
			@{ where = "\images"; recursive = $true; dest = "Content\jquery-ui\images" }
		);
		file = @(
			@{ where = ""; search = "*.css"; dest = "Content\jquery-ui" }
			@{ where = "";  search = "*.js"; dest = "Scripts\jquery-ui" }
		);
	}
	@{#jquery-ui(widget)
		module = "node_modules\jquery-ui";
		output = $local:output;
		file = @(
			@{ where = "\ui";                  search = "*.js"; dest = "Scripts\jquery-ui\widgets" }
			@{ where = "\ui\effects";          search = "*.js"; dest = "Scripts\jquery-ui\widgets" }
			@{ where = "\ui\widgets";          search = "*.js"; dest = "Scripts\jquery-ui\widgets" }
			@{ where = "\ui\i18n"; search = "datepicker-ja.js"; dest = "Scripts\jquery-ui\widgets" }
		);
	}
	@{#slickgrid
		module = "node_modules\slickgrid";
		output = $local:output;
		directory = @(
			@{ where = "\images"; recursive = $true; dest = "Content\slickgrid\images" }
			@{ where = "\images"; recursive = $true; dest = "images" } #for Slickgrid Demo
		);
		file = @(
			@{ where = "";     search = "slick*.css"; dest = "Content\slickgrid" }
			@{ where = "";      search = "slick*.js"; dest = "Scripts\slickgrid" }
			@{ where = "\controls"; search = "*.css"; dest = "Content\slickgrid\controls" }
			@{ where = "\controls";  search = "*.js"; dest = "Scripts\slickgrid\controls" }
			@{ where = "\plugins";  search = "*.css"; dest = "Content\slickgrid\plugins" }
			@{ where = "\plugins";   search = "*.js"; dest = "Scripts\slickgrid\plugins" }
			@{ where = "\lib";      search = "*.css"; dest = "Content\slickgrid\lib" }
			@{ where = "\lib";       search = "*.js"; dest = "Scripts\slickgrid\lib" }
			@{ where = "\examples"; search = "examples.css"; dest = "Content\slickgrid\examples" } #for Slickgrid Demo
			@{ where = "\examples"; search = "*.js"; dest = "Scripts\slickgrid\examples" } #for Slickgrid Demo
		);
	}
	@{#vue.js
		module = "node_modules\vue";
		output = $local:output;
		file = @(
			@{ where = "\dist";    search = "*.js"; dest = "Scripts\vue" }
		);
	}
	<# 定義サンプル
	@{
		module = "対象のモジュール名（node_modules以下のフォルダ名）";
		output = "このプロジェクトフォルダをカレントとした、出力先相対パス";
		directory = @(
			下記構造ハッシュの配列。whereと一致するフォルダ配下を、destへコピーします。recursiveが$trueのときサブフォルダも対象にします。
			@{ where = "\モジュールフォルダ配下のパス"; recursive = $true|$false; dest = "outputをカレントとしたコピー先相対パス" }
			@{ where = "空のときルート対象"; recursive = $true|$false; dest = "" }
		);
		file = @(
			下記構造ハッシュの配列。whereと一致するフォルダ配下の、searchと一致するファイルを、destへコピーします。
			@{ where = "\モジュールフォルダ配下のパス"; search = "コピー対象。ワイルドカード指定可"; dest = "outputをカレントとしたコピー先相対パス" }
			@{ where = "空のときルート対象"; search = "*.*"; dest = "" }
		);
	}
	#>
)

restore:
	mkdir ./tmp

	curl -sSL https://dot.net/v1/dotnet-install.sh >./tmp/dotnet-install.sh
	curl -sSL https://dot.net/v1/dotnet-install.asc >./tmp/dotnet-install.asc
	curl -sSL https://dot.net/v1/dotnet-install.sig >./tmp/dotnet-install.sig

	gpg --import ./tmp/dotnet-install.asc 
	gpg --verify ./tmp/dotnet-install.sig ./tmp/dotnet-install.sh
	chmod +x ./tmp/dotnet-install.sh || exit 1

	./tmp/dotnet-install.sh --jsonfile ./global.json 

	rm -r ./tmp 

	dotnet tool restore

	find . -type f -name *.*proj | xargs -I{} sh -c 'dotnet workload restore --project "{}"' 

install: restore

	rm -r ./src/Cli/bin/nupkg || true

	dotnet tool uninstall -g DotnetManageSecrets || true

	dotnet pack ./src/Cli -c Release

	dotnet tool install -g --add-source ./src/Cli/bin/nupkg DotnetManageSecrets --allow-downgrade 


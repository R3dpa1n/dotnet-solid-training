{
  description = "dev basics solid shell environment";

  inputs = {
    flake-utils.url = "github:numtide/flake-utils";
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
  };

  outputs = { self, nixpkgs, flake-utils }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
      in 
      {
        devShell =
          pkgs.mkShell {
            buildInputs = with pkgs; [
              dotnet-sdk
              omnisharp-roslyn
            ];
            shellHook = ''
            '';
          };
      });
}

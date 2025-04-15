# Schedule One URP Shader Fix

### A shader solution for Schedule I modders

Contained in this repository is the source for a simple setup script to be used in Unity Editor version 2022.3.32f1 for the purpose of automating the installation and/or configuration of URP. Without the correct URP configuration, shader compilation will be incomplete and our asset bundles won't contain the necessary shader variants for meshes to appear properly in the game. The result is that any mesh set to Opaque doesn't render when the render queue on the material is set to < 2500. Even when set higher, those and transparent materials as well will ignore lighting effects and will only be lit by the ambient color.

Install the unity package from the releases page into a fresh unity project. It's recommended to start with a Universal project (which refers to the universal render pipeline), but this script will also install and configure URP on standard Built-In projects as well. If you uninstall the URP package from the package manager after running this script, you'll have to reinstall URP, delete the contents of this package, or remove the "HAS_URP_INSTALLED" project define in the project's 'Player' settings.

Please report any issues you find, but make sure you try running this from a fresh new project before reporting anything. Installing this on existing projects may work fine, but it's unsupported.
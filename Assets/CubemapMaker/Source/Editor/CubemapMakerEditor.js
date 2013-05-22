
@CustomEditor(CubemapMaker)
class CubemapMakerEditor extends Editor
{
	var dimensionNames : String[] = ["16", "32", "64", "128", "256", "512", "1024", "2048"];
	var dimensions : int[] = [16,32,64,128,256,512,1024,2048];

	override function OnInspectorGUI()
	{
		GUILayout.Label("Asset Options", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		target.path = EditorGUILayout.TextField("Path to save in", target.path);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		target.filename = EditorGUILayout.TextField("File Name", target.filename);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		GUILayout.Label("Cubemap Options", EditorStyles.boldLabel);

		EditorGUILayout.Separator();

		EditorGUILayout.BeginHorizontal();
		target.size = EditorGUILayout.IntPopup("Cubemap Size", target.size, dimensionNames, dimensions);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		target.mipmap = EditorGUILayout.Toggle("Use Mipmaps", target.mipmap);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		GUILayout.Label("Camera Options", EditorStyles.boldLabel);

		EditorGUILayout.Separator();

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Use Background Color instead of Skybox");
		target.cameraOptions.useBackground = EditorGUILayout.Toggle(target.cameraOptions.useBackground);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(target.cameraOptions.useBackground)
		{
			target.cameraOptions.background = EditorGUILayout.ColorField("Background", target.cameraOptions.background);
		}
		else
		{
			GUILayout.Label("Rendering with Skybox.");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();
		target.cameraOptions.farClipping = EditorGUILayout.Slider("Far Clipping", target.cameraOptions.farClipping, 1, 10000);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Separator();

		GUILayout.Label("Info", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("- Please ensure that your GameView is more wide than tall.\n- Screen.height should be >= Cubemap Size for good results.\n- Start the game and press F12 to create the cubemap.\n- Get results faster with all Cameras turned off.");
		EditorGUILayout.EndHorizontal();
	}
}
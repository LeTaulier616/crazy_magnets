#pragma strict

class CameraOptions
{
	//var clearFlags : CameraClearFlags;
	var useBackground : boolean = false;
	var background : Color = Color.black;
	//var cullingMask : int;
	var farClipping : float = 1000;
}

var path : String = "CubemapMaker/CreatedCubemap";
var filename : String = "CreatedCubemap";
var size : int = 256;
private var textureFormat : TextureFormat = TextureFormat.ARGB32;
var mipmap : boolean = false;

var cameraOptions : CameraOptions = CameraOptions();

function Update()
{
	if(Application.isEditor && Input.GetKeyDown(KeyCode.F12))
	{
		CreateCubemap();
	}
}

function CreateCubemap()
{
	size = Mathf.ClosestPowerOfTwo(size);
	var camObject : GameObject = GameObject("CubemapCamera");
	camObject.transform.parent = transform;
	camObject.transform.localPosition = Vector3.zero;
	var cam : Camera = camObject.AddComponent(Camera);
	cam.fieldOfView = 90;
	cam.depth = -10;
	cam.nearClipPlane = 0.001;
	if(cameraOptions.useBackground)
	{
		cam.clearFlags = CameraClearFlags.SolidColor;
	}
	cam.backgroundColor = cameraOptions.background;
	cam.farClipPlane = cameraOptions.farClipping;
    
    //deactivate main camera....
    if(Camera.main)
        Camera.main.enabled = false;

	if(cam.aspect < 1.0)
	{
		Debug.LogWarning("Ensure that your Game View is more wide than tall!");
	}
	else
	{
		var cubemap : Cubemap = Cubemap(size, textureFormat, mipmap);
		yield Snapshot(cubemap, CubemapFace.PositiveZ, cam);
		yield Snapshot(cubemap, CubemapFace.PositiveX, cam);
		yield Snapshot(cubemap, CubemapFace.NegativeX, cam);
		yield Snapshot(cubemap, CubemapFace.NegativeZ, cam);
		yield Snapshot(cubemap, CubemapFace.PositiveY, cam);
		yield Snapshot(cubemap, CubemapFace.NegativeY, cam);
		cubemap.Apply(mipmap);

		AssetDatabase.CreateAsset(cubemap, "Assets/"+path+"/"+filename+".cubemap");
	}
	Destroy(camObject);

    if(Camera.main)
        Camera.main.enabled = true;
}

private function Snapshot(cubemap : Cubemap, face : CubemapFace, cam : Camera)
{
	var width = Screen.width;
	var height = Screen.height;
	var tex = Texture2D(height, height, textureFormat, mipmap);

	cam.transform.localRotation = RotationOf(face);
	yield WaitForEndOfFrame();

	tex.ReadPixels(Rect((width-height)/2, 0, height, height), 0, 0);
	tex.Apply();
	tex = Scale(tex, size, size);

	var colors : Color[] = tex.GetPixels();
	for(var i : int = 0; i < colors.length; i++)
	{
		cubemap.SetPixel(face, size-(i%size)-1, Mathf.Floor(i / size), colors[colors.length-i-1]);
	}
}

private function RotationOf(face : CubemapFace) : Quaternion
{
	var result : Quaternion;
	switch(face)
	{
		case CubemapFace.PositiveX:
			result = Quaternion.Euler(0,90,0);
		break;
		case CubemapFace.NegativeX:
			result = Quaternion.Euler(0,-90,0);
		break;
		case CubemapFace.PositiveY:
			result = Quaternion.Euler(-90,0,0);
		break;
		case CubemapFace.NegativeY:
			result = Quaternion.Euler(90,0,0);
		break;
		case CubemapFace.NegativeZ:
			result = Quaternion.Euler(0,180,0);
		break;
		default:
			result = Quaternion.identity;
		break;
	}
	return result;
}

function OnDrawGizmos()
{
	//Gizmos.DrawIcon(transform.position, "CubemapMakerIcon.tif");
	Gizmos.DrawSphere(transform.position, 0.4);
}

// Code taken from Jon-Martin.com
static function Scale(source : Texture2D, targetWidth : int, targetHeight : int) : Texture2D
{
    var result = Texture2D(targetWidth,targetHeight,source.format,true);
    var rpixels = result.GetPixels(0);
    var incX = (1f/source.width)*((source.width *1f)/targetWidth);
    var incY = (1f/source.height)*((source.height * 1f)/targetHeight);
    for(var px = 0; px < rpixels.Length; px++)
    {
        rpixels[px] = source.GetPixelBilinear(incX*(px%targetWidth),
                             incY*(Mathf.Floor(px/targetWidth)));
    }
    result.SetPixels(rpixels,0);
    result.Apply();
    return result;
}
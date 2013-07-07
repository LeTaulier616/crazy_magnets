using UnityEngine;
using System.Collections;

public class TextureAnimator : MonoBehaviour {
	
	public int UVTileGrabX, UVTileGrabY, FPSGrab;
	public int UVTileBoltX, UVTileBoltY, FPSBolt;
	public Material BoltLine, GrabLine;
	
    private float iX=0;
    private float iY=1;
    public int _uvTieX;
    public int _uvTieY;
    public int _fps;
    private Vector2 _size;
    private Renderer _myRenderer;
    private int _lastIndex = -1;
	
	private Controllable player;
 
    void Start ()
    {		
		player = GlobalVarScript.instance.player.GetComponent<Controllable>();

		_uvTieX = UVTileBoltX;
		_uvTieY = UVTileBoltY;
		_fps = FPSBolt;
		
 
        _myRenderer = renderer;
 
        if(_myRenderer == null) enabled = false;		
    }
 
 
 
    void Update()
    {		
        int index = (int)(Time.timeSinceLevelLoad * _fps) % (_uvTieX * _uvTieY);
 		
		_size = new Vector2 (1.0f / _uvTieX , 1.0f / _uvTieY);
			
        if(index != _lastIndex)
        {
            Vector2 offset = new Vector2(iX * _size.x,
                                         1 - (_size.y * iY));
            iX++;
			
            if(iX / _uvTieX == 1)
            {
                if(_uvTieY!=1)    iY++;
				
                iX=0;
				
                if(iY / _uvTieY == 1)
                {
                    iY=1;
                }
            }
 
            _myRenderer.material.SetTextureOffset ("_MainTex", offset);
			_myRenderer.material.SetTextureScale ("_MainTex", _size);
 
            _lastIndex = index;
        }
    }
	
	public void SetForBolt()
	{
		_uvTieX = UVTileBoltX;
		_uvTieY = UVTileBoltY;
		_fps = FPSBolt;
		
		if(_myRenderer.material != BoltLine)
			_myRenderer.material = BoltLine;
	}
	
	public void SetForGrab()
	{
		_uvTieX = UVTileGrabX;
		_uvTieY = UVTileGrabY;
		_fps = FPSGrab;
		
		if(_myRenderer.material != GrabLine)
			_myRenderer.material = GrabLine;
	}
}

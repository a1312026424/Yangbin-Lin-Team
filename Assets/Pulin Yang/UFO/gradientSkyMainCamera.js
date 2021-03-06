var topColor = Color.blue;
var bottomColor = Color.white;
var gradientLayer = 7;
 
function Awake () {
	gradientLayer = Mathf.Clamp(gradientLayer, 0, 31);
	if (!GetComponent.<Camera>()) {
		Debug.LogError ("Must attach GradientBackground script to the camera");
		return;
	}
 
	GetComponent.<Camera>().clearFlags = CameraClearFlags.Depth;
	GetComponent.<Camera>().cullingMask = GetComponent.<Camera>().cullingMask & ~(1 << gradientLayer);
	var gradientCam = new GameObject("Gradient Cam", Camera).GetComponent.<Camera>();
	gradientCam.depth = GetComponent.<Camera>().depth-1;
	gradientCam.cullingMask = 1 << gradientLayer;
 
	var mesh = new Mesh();
	mesh.vertices = [Vector3(-100, .577, 1), Vector3(100, .577, 1), Vector3(-100, -.577, 1), Vector3(100, -.577, 1)];
	mesh.colors = [topColor, topColor, bottomColor, bottomColor];
	mesh.triangles = [0, 1, 2, 1, 3, 2];
 
	var mat = new Material("Shader \"Vertex Color Only\"{Subshader{BindChannels{Bind \"vertex\", vertex Bind \"color\", color}Pass{}}}");
	var gradientPlane = new GameObject("Gradient Plane", MeshFilter, MeshRenderer);
	(gradientPlane.GetComponent(MeshFilter) as MeshFilter).mesh = mesh;
	gradientPlane.GetComponent.<Renderer>().material = mat;
	gradientPlane.layer = gradientLayer;
}
using UnityEngine;
using System.Collections;

namespace DistanceFieldSystem {

	public class DistanceField : MonoBehaviour {
		public enum DebugModeEnum { Default = 0, FullScreen }
		public int lod = 1;
		public int pointCapacity = 64;
		public DebugModeEnum debugMode;
		public Camera targetCamera;
		public Material distMat;
		public Transform[] points;

		RenderTexture _distTex;
		RenderTexture _normTex;
		PointService _points;
		Renderer _renderer;
		MaterialPropertyBlock _props;

		public void FitAspect() {
			var s = transform.localScale;
			s.x = s.y * targetCamera.aspect;
			transform.localScale = s;
		}

		void Start() {
			_props = new MaterialPropertyBlock();
			_renderer = GetComponent<Renderer>();
			_renderer.SetPropertyBlock(_props);
		}
		void Update() {
			CheckInit();
			_points.Update(points, targetCamera);
			_points.SetData(distMat);
			SetData(distMat);
			Graphics.Blit(null, _distTex, distMat, 0);
			Graphics.Blit(_distTex, _normTex, distMat, 1);
			_renderer.SetPropertyBlock(_props);

			DebugMode();
		}
		void OnDestroy() {
			Release();
		}
		void OnDrawGizmos() {
			if (_points != null)
				_points.DrawGizmos(targetCamera);
		}

		void CheckInit() {
			var width = targetCamera.pixelWidth >> lod;
			var height = targetCamera.pixelHeight >> lod;

			if (_distTex == null || _distTex.width != width || _distTex.height != height) {
				Release();
				_distTex = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
				_distTex.filterMode = FilterMode.Bilinear;
				_distTex.wrapMode = TextureWrapMode.Clamp;
				_distTex.antiAliasing = (QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing);
				_props.SetTexture(ShaderConsts.PROP_DIST_TEXTURE, _distTex);

				_normTex = new RenderTexture(width, height, 0, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear);
				_normTex.filterMode = FilterMode.Bilinear;
				_normTex.wrapMode = TextureWrapMode.Clamp;
				_normTex.antiAliasing = _distTex.antiAliasing;
				_props.SetTexture(ShaderConsts.PROP_NORM_TEXTURE, _normTex);

				var r = Mathf.Sqrt(width * height);
				//_props.SetVector(ShaderConsts.PROP_VIEW_SCALE, 
				//                 new Vector4(20f / r, 1f * r, 0, 0));
			}
			if (_points == null) {
				_points = new PointService(pointCapacity);
			}
		}
		void SetData(Material m) {
			var width = targetCamera.pixelWidth;
			var height = targetCamera.pixelHeight;
			m.SetVector(ShaderConsts.PROP_SCREEN_SIZE, new Vector4(width, height, 1f/width, 1f/height));
		}
		void DebugMode() {
			switch (debugMode) {
			case DebugModeEnum.FullScreen:
				var size = 2f * targetCamera.orthographicSize;
				transform.position = Vector3.MoveTowards(targetCamera.transform.position, targetCamera.transform.forward, targetCamera.nearClipPlane);
				transform.forward = targetCamera.transform.forward;
				transform.localScale = new Vector3(targetCamera.aspect * size, size, 1f);
				break;
			default:
				break;
			}
		}
		void Release() {
			if (_distTex != null) {
				Destroy(_distTex);
				_distTex = null;
			}
			if (_normTex != null) {
				Destroy(_normTex);
				_normTex = null;
			}
			if (_points != null) {
				_points.Dispose();
				_points = null;
			}
		}
	}
}
using UnityEngine;
using System.Collections;

namespace DistanceFieldSystem {

	public class DistanceField : MonoBehaviour {
		public enum DebugModeEnum { Normal = 0, Debug }
		public enum HeightModeEnum { Distance = 0, Metaball }
		[System.Flags]
		public enum ViewModeEnum { Normal = 0, Height, LIC, Flow }
		public int lod = 1;
		public int pointCapacity = 64;
		public DebugModeEnum debugMode;
		public HeightModeEnum heightMode;
		public ViewModeEnum viewMode;
		public Camera targetCamera;
		public Material distMat;
		public Transform[] points;

		RenderTexture _distTex;
		RenderTexture _normTex;
		PointService _points;
		Renderer _renderer;
		MaterialPropertyBlock _viewProps;

		public void FitCamera() {
			var posViewport = new Vector3 (0.5f, 0.5f, targetCamera.farClipPlane - Mathf.Epsilon);
			transform.position = targetCamera.ViewportToWorldPoint (posViewport);
			transform.rotation = targetCamera.transform.rotation;
			var size = 2f * targetCamera.orthographicSize;
			transform.localScale = new Vector3 (size * targetCamera.aspect, size, 1f);
		}

		void Start() {
			_viewProps = new MaterialPropertyBlock();
			_renderer = GetComponent<Renderer>();
			_renderer.SetPropertyBlock(_viewProps);
		}
		void Update() {
			CheckInit();
			_points.Update(points, targetCamera);
			_points.SetData(distMat);
			SetData(distMat);
			Graphics.Blit(null, _distTex, distMat, 0);
			Graphics.Blit(_distTex, _normTex, distMat, 1);
			_renderer.SetPropertyBlock(_viewProps);

			SetViewMode();
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
				_distTex = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
				_distTex.filterMode = FilterMode.Bilinear;
				_distTex.wrapMode = TextureWrapMode.Clamp;
				_distTex.antiAliasing = (QualitySettings.antiAliasing == 0 ? 1 : QualitySettings.antiAliasing);
				_viewProps.SetTexture(ShaderConsts.PROP_DIST_TEXTURE, _distTex);

				_normTex = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
				_normTex.filterMode = FilterMode.Bilinear;
				_normTex.wrapMode = TextureWrapMode.Clamp;
				_normTex.antiAliasing = _distTex.antiAliasing;
				_viewProps.SetTexture(ShaderConsts.PROP_NORM_TEXTURE, _normTex);
			}
			if (_points == null) {
				_points = new PointService(pointCapacity);
			}
		}
		void SetData(Material m) {
			var width = targetCamera.pixelWidth;
			var height = targetCamera.pixelHeight;
			m.SetVector(ShaderConsts.PROP_SCREEN_SIZE, new Vector4(width, height, 1f/width, 1f/height));

			var r = Mathf.Sqrt(width * height);
			m.shaderKeywords = null;
			switch (heightMode) {
			case HeightModeEnum.Metaball:
				m.EnableKeyword(ShaderConsts.KEY_FIELD_METABALL);
				m.SetFloat(ShaderConsts.PROP_DIST_SCALE, 1f * r);
				break;
			default:
				m.EnableKeyword(ShaderConsts.KEY_FIELD_DISTANCE);
				m.SetFloat(ShaderConsts.PROP_DIST_SCALE, 5f / r);			
				break;
			}
		}
		void SetViewMode() {
			switch (debugMode) {
			case DebugModeEnum.Debug:
				_renderer.enabled = true;
				break;
			default:
				_renderer.enabled = false;
				break;
			}

			switch (viewMode) {
			case ViewModeEnum.Height:
				_viewProps.SetVector(ShaderConsts.PROP_VIEW_FILTER, new Vector4(0, 1, 0, 0));
				break;
			case ViewModeEnum.LIC:
				_viewProps.SetVector(ShaderConsts.PROP_VIEW_FILTER, new Vector4(0, 0, 1, 0));
				break;
			case ViewModeEnum.Flow:
				_viewProps.SetVector(ShaderConsts.PROP_VIEW_FILTER, new Vector4(0, 0, 0, 1));
				break;
			default:
				_viewProps.SetVector(ShaderConsts.PROP_VIEW_FILTER, new Vector4(1, 0, 0, 0));
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
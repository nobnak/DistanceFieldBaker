using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DistanceFieldSystem {

	public class PointService : System.IDisposable {
		int _capacity;
		int _count;
		ComputeBuffer _points;
		Vector2[] _pointData;

		public PointService(int capacity) {
			_capacity = capacity;
			_count = 0;
			_pointData = new Vector2[capacity];
			_points = new ComputeBuffer(capacity, Marshal.SizeOf(typeof(Vector2)));
			Upload();
		}
		public void Update(IList<Transform> points, Camera cam) {
			_count = Mathf.Min(_capacity, points.Count);
			for (var i = 0; i < _count; i++)
				_pointData[i] = (Vector2)cam.WorldToScreenPoint(points[i].position);
			Upload();
		}
		public void Upload() { _points.SetData(_pointData); }
		public void Download() { 
			_points.GetData(_pointData);
		}
		public void SetData(ComputeShader c, int k) {
			c.SetInt(ShaderConsts.PROP_POINT_COUNT, _count);
			c.SetBuffer(k, ShaderConsts.BUF_POINT, _points);
		}
		public void SetData(Material m) {
			m.SetInt(ShaderConsts.PROP_POINT_COUNT, _count);
			m.SetBuffer(ShaderConsts.BUF_POINT, _points);
		}
		public void DrawGizmos(Camera cam) {
			var size = 0.1f * Vector3.one;
			Gizmos.color = Color.green;
			for (var i = 0; i < _count; i++) {
				var posScreen = (Vector3)_pointData[i];
				posScreen.z = cam.nearClipPlane;
				Gizmos.DrawCube(cam.ScreenToWorldPoint(posScreen), size);
			}
		}

		#region IDisposable implementation
		public void Dispose () {
			if (_points != null)
				_points.Dispose();
		}
		#endregion
	}
}
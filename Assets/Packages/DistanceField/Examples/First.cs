using UnityEngine;
using System.Collections;

namespace DistanceFieldSystem {

	public class First : MonoBehaviour {
		public DistanceField distField;
		public int count = 64;
		public float radius = 10f;
		public GameObject pointfab;

		public Vector3 angularVelocity;

		Transform[] _points;

		void Start() {
			_points = new Transform[count];
			for (var i = 0; i < count; i++) {
				var pos = radius * Random.insideUnitSphere;
				var p = Instantiate(pointfab);
				p.transform.SetParent(transform);
				p.transform.localPosition = pos;
				_points[i] = p.transform;
			}
			distField.points = _points;
		}
		void Update() {
			transform.localRotation *= Quaternion.Euler(angularVelocity * Time.deltaTime);
		}
	}
}
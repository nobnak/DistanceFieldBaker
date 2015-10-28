using UnityEngine;
using System.Collections;

namespace DistanceFieldSystem {

	public static class ShaderConsts {
		public const string PROP_DIST_TEXTURE = "_DistTex";
		public const string PROP_NORM_TEXTURE = "_NormTex";
		public const string PROP_POINT_COUNT = "PointCount";
		public const string PROP_SCREEN_SIZE = "ScreenSize";
		public const string PROP_DIST_SCALE = "_Scale";
		public const string PROP_VIEW_FILTER = "_Filter";

		public const string BUF_POINT = "PointBuf";

		public const string KEY_FIELD_DISTANCE = "DISTANCE_FIELD";
		public const string KEY_FIELD_METABALL = "METABALL_FIELD";

		public const string KEY_VIEW_HGIEHT = "HEIGHT_FIELD";
		public const string KEY_VIEW_NORMAL = "NORMAL_FIELD";
	}
}
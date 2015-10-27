using UnityEngine;
using System.Collections;

namespace DistanceFieldSystem {

	public static class ShaderConsts {
		public const string PROP_DIST_TEXTURE = "_DistTex";
		public const string PROP_NORM_TEXTURE = "_NormTex";
		public const string PROP_POINT_COUNT = "PointCount";
		public const string PROP_SCREEN_SIZE = "ScreenSize";
		public const string PROP_VIEW_SCALE = "_Scale";

		public const string BUF_POINT = "PointBuf";

		public const string KEY_FIT_SCREEN_ON = "FIT_SCREEN_ON";
	}
}
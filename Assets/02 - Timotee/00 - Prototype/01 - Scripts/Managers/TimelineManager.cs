using UnityEngine;
using UnityEngine.Playables;

namespace Managers{
	public class TimelineManager : MonoBehaviour{
		public static TimelineManager Instance;

		[SerializeField] private PlayableAsset _arrested;
		private PlayableDirector _director;

		private void Awake(){
			if (Instance==null) Instance = this;
			_director = GetComponent<PlayableDirector>();
		}

		public static void Arrest(){
			Instance.LaunchArrestedTimeline();
		}

		private void LaunchArrestedTimeline(){
			_director.Play(_arrested);
		}
	}
}
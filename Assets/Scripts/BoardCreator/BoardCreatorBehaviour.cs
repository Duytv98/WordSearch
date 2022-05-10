using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreatorBehaviour : MonoBehaviour
	{
		#region Unity Methods
		private void Update()
		{
			// kiểm tra BoardCreator đã hoàn thành chưa, nếu đã hoàn thành thì gọi OnBoardWorkerFinished
			if (BoardCreator.IsFinished)
			{
				BoardCreator.OnBoardWorkerFinished();
			}
		}

		#endregion
	}

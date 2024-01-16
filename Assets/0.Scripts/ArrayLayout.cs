using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout
{

	[System.Serializable]
	public struct rowData
	{
		public bool[] row;
	}

	public rowData[] rows = new rowData[8]; //Y가 8인 grid를 만든다 CustPropertyDrawer.cs에 의해 제어됨

}
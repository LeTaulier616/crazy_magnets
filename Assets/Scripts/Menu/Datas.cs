using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

public class Datas
{
	/****************************************************************************************/
	/****************************************************************************************/
	/********  Always get or set shared_datas via Datas.sharedDatas() function      *********/
	/********  Return shared_datas if exists, otherwise create a new Datas instance *********/
	/****************************************************************************************/
	/****************************************************************************************/
	
	private static Datas  shared_datas = null;
	private static string directory    = Application.persistentDataPath + "/save";
	private static string path         = directory + "/drmadget.txt";
	
	public static Datas sharedDatas()
	{
		if(shared_datas == null)
		{
			if(!System.IO.Directory.Exists(directory))
			{
				System.IO.Directory.CreateDirectory(directory);
		    }
			shared_datas = new Datas();
			if(System.IO.File.Exists(path))
				shared_datas.loadDatas();
			Debug.Log(path);
		}
		return shared_datas;
	}
	
	
	/****************************/
	/****** Datas to Save *******/
	
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct DatasStruct {
		
		// Level Actif (Level we are playing in Story)
		public int currentWorld;
		public int currentLevel;
		
		// Last Level Unlocked
		public int lastWorld;
		public int lastLevel;
		
		// Selected Level (for Menus and Launch correct Level)
		public int selectedLevel;
		public int selectedWorld;
		
		public int globalVolume;
		public int sfxVolume;
		public int bgmVolume;
		
		public bool    isNewGame;
		
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] // Max length of array
		public float[] timeLevels;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] // Max length of array
		public int[]   screwsGotchaByLevel;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] // Max length of array
		public bool[]  screwsGotcha;
	};
	
	public DatasStruct datas;
	
	/****** Datas to Save *******/
	/****************************/
	
	
	Datas()
	{
		reinitDatas();
	}
	
	private void reinitDatas()
	{
		datas.currentWorld  = 0;
		datas.currentLevel  = 0;
		
		datas.lastWorld     = 0;
		datas.lastLevel     = 5;
		
		datas.selectedLevel = 0;
		datas.selectedWorld = 0;
	
		datas.globalVolume  = 100;
		datas.sfxVolume     = 100;
		datas.bgmVolume     = 100;
		
		datas.isNewGame     = true;
		
		datas.timeLevels = new float[MyDefines.kNbLevels];
		for(int iii = 0; iii < MyDefines.kNbLevels; ++iii)
			datas.timeLevels[iii] = 0;
		
		datas.screwsGotchaByLevel = new int[MyDefines.kNbLevels];
		for(int iii = 0; iii < MyDefines.kNbLevels; ++iii)
			datas.screwsGotchaByLevel[iii] = 0;
		
		datas.screwsGotcha = new bool[MyDefines.kNbScrews];
		for(int iii = 0; iii < MyDefines.kNbScrews; ++iii)
			datas.screwsGotcha[iii] = false;
	}
	
	public void loadDatas()
	{
		byte[] bytes = File.ReadAllBytes(path);
		object d = DeserializeMsg<DatasStruct>(bytes);
		datas = (DatasStruct)d;
	}
	
	public void saveDatas()
	{
		byte[] bytes = SerializeMessage<DatasStruct>(datas);
		File.WriteAllBytes(path, bytes);
	}
	
	// Methode 1
	public static byte[] getBytes(object o)
    {
        int size = Marshal.SizeOf(o);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(o, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);
        return arr;
    }

	public static object getStruct(byte[] arr, object o)
	{
		//object str = new object();
		int size = Marshal.SizeOf(o);
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.Copy(arr, 0, ptr, size);
		o = (object)Marshal.PtrToStructure(ptr, o.GetType());
		Marshal.FreeHGlobal(ptr);
		return o;
	}
	
	// Methode 2
	public static byte[] SerializeMessage<T>(T msg) where T : struct
	{
		int objsize = Marshal.SizeOf(typeof(T));
		byte[] ret = new byte[objsize];
		IntPtr buff = Marshal.AllocHGlobal(objsize);
		Marshal.StructureToPtr(msg, buff, true);
		Marshal.Copy(buff, ret, 0, objsize);
		Marshal.FreeHGlobal(buff);
		return ret;
	}
	
	public static T DeserializeMsg<T>(byte[] data) where T : struct
	{
		int objsize = Marshal.SizeOf(typeof(T));
		IntPtr buff = Marshal.AllocHGlobal(objsize);
		Marshal.Copy(data, 0, buff, objsize);
		T retStruct = (T)Marshal.PtrToStructure(buff, typeof(T));
		Marshal.FreeHGlobal(buff);
		return retStruct;
	}
}

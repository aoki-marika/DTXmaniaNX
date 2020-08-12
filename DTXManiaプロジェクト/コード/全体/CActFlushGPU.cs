﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using SlimDX.Direct3D9;
using FDK;

namespace DTXMania
{
	/// <summary>
	/// 描画フレーム毎にGPUをフラッシュして、描画遅延を防ぐ。
	/// DirectX9の、Occlusion Queryを用いる。(Flush属性付きでGetDataする)
	/// Device Lost対策のため、QueueをCActivitiyのManagedリソースとして扱う。
	/// On進行描画()を呼び出すことで、GPUをフラッシュする。
	/// </summary>
	internal class CActFlushGPU : CActivity
	{
		// CActivity 実装

		public override void OnManagedCreateResources()
		{
			if ( !base.bNotActivated )
			{
				try			// #xxxxx 2012.12.31 yyagi: to prepare flush, first of all, I create q queue to the GPU.
				{
					IDirect3DQuery9 = new SlimDX.Direct3D9.Query( CDTXMania.app.Device, QueryType.Occlusion );
				}
				catch ( Exception e )
				{
					Trace.TraceError( e.Message );
				}
				base.OnManagedCreateResources();
			}
		}
		public override void  OnManagedReleaseResources()
		{
			IDirect3DQuery9.Dispose();
			IDirect3DQuery9 = null;
			base.OnManagedReleaseResources();
		}
		public override int On進行描画()
		{
			if ( !base.bNotActivated )
			{
				IDirect3DQuery9.Issue( Issue.End );
				DWM.Flush();
				IDirect3DQuery9.GetData<int>( true );	// flush GPU queue
			}
			return 0;
		}

		// その他

		#region [ private ]
		//-----------------
		private SlimDX.Direct3D9.Query IDirect3DQuery9;
		//-----------------
		#endregion
	}
}

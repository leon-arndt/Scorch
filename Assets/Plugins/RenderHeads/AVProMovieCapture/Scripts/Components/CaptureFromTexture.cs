#if UNITY_5_4_OR_NEWER
	#define AVPRO_MOVIECAPTURE_RENDERTEXTUREBGRA32_54
#endif
using UnityEngine;

//-----------------------------------------------------------------------------
// Copyright 2012-2017 RenderHeads Ltd.  All rights reserved.
//-----------------------------------------------------------------------------

namespace RenderHeads.Media.AVProMovieCapture
{
	/// <summary>
	/// Capture from a Texture object (including RenderTexture, WebcamTexture)
	/// </summary>
	[AddComponentMenu("AVPro Movie Capture/Capture From Texture", 3)]
	public class CaptureFromTexture : CaptureBase
	{
		private Texture _sourceTexture;
		private RenderTexture _renderTexture;
		private System.IntPtr _targetNativePointer = System.IntPtr.Zero;

		public void SetSourceTexture(Texture texture)
		{
			_sourceTexture = texture;

			if (texture is Texture2D)
			{
				if ((((Texture2D)texture).format != TextureFormat.ARGB32) &&
					(((Texture2D)texture).format != TextureFormat.RGBA32) &&
					(((Texture2D)texture).format != TextureFormat.BGRA32))
				{
					Debug.LogWarning("[AVProMovieCapture] texture format may not be supported: " + ((Texture2D)texture).format);
				}
			}
			else if (texture is RenderTexture)
			{
				if ((((RenderTexture)texture).format != RenderTextureFormat.ARGB32) &&
					(((RenderTexture)texture).format != RenderTextureFormat.Default)
#if AVPRO_MOVIECAPTURE_RENDERTEXTUREBGRA32_54
				&& (((RenderTexture)texture).format != RenderTextureFormat.BGRA32)
#endif
				)
				{
					Debug.LogWarning("[AVProMovieCapture] texture format may not be supported: " + ((RenderTexture)texture).format);
				}
			}
		}

		public override void UpdateFrame()
		{
			TickFrameTimer();

			if (_capturing && !_paused && _sourceTexture)
			{
				bool canGrab = true;

				// If motion blur is enabled, wait until all frames are accumulated
				if (IsUsingMotionBlur())
				{
					// If the motion blur is still accumulating, don't grab this frame
					canGrab = _motionBlur.IsFrameAccumulated;
				}

				if (canGrab)
				{
					if (CanOutputFrame())
					{
						// If motion blur is enabled, use the motion blur result
						Texture sourceTexture = _sourceTexture;
						if (IsUsingMotionBlur())
						{
							sourceTexture = _motionBlur.FinalTexture;
						}

						// If the texture isn't a RenderTexture then blit it to the Rendertexture so the native plugin can grab it
						if (!(sourceTexture is RenderTexture))
						{
							_renderTexture.DiscardContents();
							Graphics.Blit(sourceTexture, _renderTexture);
							sourceTexture = _renderTexture;
						}

						if (_targetNativePointer == System.IntPtr.Zero || _supportTextureRecreate)
						{
							// NOTE: If support for captures to survive through alt-tab events, or window resizes where the GPU resources are recreated
							// is required, then this line is needed.  It is very expensive though as it does a sync with the rendering thread.
							_targetNativePointer = sourceTexture.GetNativeTexturePtr();
						}

						NativePlugin.SetTexturePointer(_handle, _targetNativePointer);

						RenderThreadEvent(NativePlugin.PluginEvent.CaptureFrameBuffer);

						// Handle audio from Unity
						if (IsRecordingUnityAudio())
						{
							int audioDataLength = 0;
							System.IntPtr audioDataPtr = _audioCapture.ReadData(out audioDataLength);
							if (audioDataLength > 0)
							{
								NativePlugin.EncodeAudio(_handle, audioDataPtr, (uint)audioDataLength);
							}
						}

						UpdateFPS();
					}
				}
			}
			base.UpdateFrame();

			RenormTimer();
		}

		private void LateUpdate()
		{
			if (_motionBlur != null)
			{
				if (_capturing && !_paused && _handle >= 0)
				{
					_motionBlur.Accumulate(_sourceTexture);
				}
			}
		}

		public override Texture GetPreviewTexture()
		{
			if (IsUsingMotionBlur())
			{
				return _motionBlur.FinalTexture;
			}
			if (_sourceTexture is RenderTexture)
			{
				return _sourceTexture;
			}
			return _renderTexture;
		}

		public override bool PrepareCapture()
		{
			if (_capturing)
			{
				return false;
			}

			if (_sourceTexture == null)
			{
				Debug.LogError("[AVProMovieCapture] No texture set to capture");
				return false;
			}

			if (SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL") && !SystemInfo.graphicsDeviceVersion.Contains("emulated"))
			{
				Debug.LogError("[AVProMovieCapture] OpenGL not yet supported for CaptureFromTexture component, please use Direct3D11 instead. You may need to switch your build platform to Windows.");
				return false;
			}

			_pixelFormat = NativePlugin.PixelFormat.RGBA32;

			SelectRecordingResolution(_sourceTexture.width, _sourceTexture.height);

			_renderTexture = RenderTexture.GetTemporary(_targetWidth, _targetHeight, 0, RenderTextureFormat.ARGB32);
			_renderTexture.Create();

			GenerateFilename();

			return base.PrepareCapture();
		}

		public override void UnprepareCapture()
		{
			_targetNativePointer = System.IntPtr.Zero;
			NativePlugin.SetTexturePointer(_handle, System.IntPtr.Zero);

			if (_renderTexture != null)
			{
				RenderTexture.ReleaseTemporary(_renderTexture);
				_renderTexture = null;
			}

			base.UnprepareCapture();
		}
	}
}
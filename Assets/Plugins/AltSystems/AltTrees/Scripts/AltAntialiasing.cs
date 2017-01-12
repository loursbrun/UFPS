using System;
using UnityEngine;

namespace AltSystems.AltTrees
{
	[ExecuteInEditMode]
	[RequireComponent(typeof (Camera))]
	public class AltAntialiasing : PostEffectsBase
	{
		public bool showGeneratedNormals = false;
		public float offsetScale = 0.2f;
		public float blurRadius = 18.0f;
		
		public float edgeThresholdMin = 0.05f;
		public float edgeThreshold = 0.2f;
		public float edgeSharpness = 4.0f;
		
		public bool dlaaSharp = false;

		public Shader shaderFXAAIII;
		private Material materialFXAAIII;
		
		
		public Material CurrentAAMaterial()
		{
			Material returnValue = materialFXAAIII;
			
			return returnValue;
		}
		
		
		public override bool CheckResources()
		{
			CheckSupport(false);

			materialFXAAIII = CreateMaterial(shaderFXAAIII, materialFXAAIII);

			return isSupported;
		}
		
		
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			
			
			RenderTexture.active = source;
			Texture2D virtualPhoto = new Texture2D(source.width, source.height, TextureFormat.ARGB32, false);
			virtualPhoto.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
			virtualPhoto.Apply();

			
			Color32[] cols = virtualPhoto.GetPixels32();
			Color32 colTemp = Color.white;
			bool isCol = false;

			for(int i=0;i<source.height;i++)
			{
				isCol = false;
				for(int j=0;j<source.width;j++)
				{
					if(isCol)
					{
						if(cols[i*source.height+j].Equals(new Color32(255,0,255,255)))
							cols[i*source.height+j] = colTemp;
						else
						{
							colTemp = cols[i*source.height+j];
							colTemp.a = 0;
							isCol = true;
						}
					}
					else
					{
						if(!cols[i*source.height+j].Equals(new Color32(255,0,255,255)))
						{
							colTemp = cols[i*source.height+j];
							colTemp.a = 0;
							isCol = true;
						}
					}
				}
				isCol = false;
				for(int j=source.width-1;j>=0;j--)
				{
					if(isCol)
					{
						if(cols[i*source.height+j].Equals(new Color32(255,0,255,255)))
							cols[i*source.height+j] = colTemp;
						else
						{
							colTemp = cols[i*source.height+j];
							colTemp.a = 0;
							isCol = true;
						}
					}
					else
					{
						if(!cols[i*source.height+j].Equals(new Color32(255,0,255,255)))
						{
							colTemp = cols[i*source.height+j];
							colTemp.a = 0;
							isCol = true;
						}
					}
				}
			}
			
			isCol = false;
			
			for(int i=0;i<source.width;i++)
			{
				isCol = false;
				for(int j=0;j<source.height;j++)
				{
					if(isCol)
					{
						if(cols[j*source.width+i].Equals(new Color32(255,0,255,255)))
							cols[j*source.width+i] = colTemp;
						else
						{
							colTemp = cols[j*source.width+i];
							colTemp.a = 0;
							isCol = true;
						}
					}
					else
					{
						if(!cols[j*source.width+i].Equals(new Color32(255,0,255,255)))
						{
							colTemp = cols[j*source.width+i];
							colTemp.a = 0;
							isCol = true;
						}
					}
				}
				isCol = false;
				for(int j=source.height-1;j>=0;j--)
				{
					if(isCol)
					{
						if(cols[j*source.width+i].Equals(new Color32(255,0,255,255)))
							cols[j*source.width+i] = colTemp;
						else
						{
							colTemp = cols[j*source.width+i];
							colTemp.a = 0;
							isCol = true;
						}
					}
					else
					{
						if(!cols[j*source.width+i].Equals(new Color32(255,0,255,255)))
						{
							colTemp = cols[j*source.width+i];
							colTemp.a = 0;
							isCol = true;
						}
					}
				}
			}


			virtualPhoto.SetPixels32(cols);
			virtualPhoto.Apply();

			
			RenderTexture.active = null; 
			
			
			
			
			
			
			if (CheckResources() == false)
			{
				Graphics.Blit(virtualPhoto, destination);
				return;
			}




			
			// ----------------------------------------------------------------
			// FXAA antialiasing modes
			
			if (materialFXAAIII != null)
			{
				materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
				materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
				materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);
				
				Graphics.Blit(virtualPhoto, destination, materialFXAAIII);
			}
			else
			{
				// none of the AA is supported, fallback to a simple blit
				Graphics.Blit(virtualPhoto, destination);
			}
		}
	}
}

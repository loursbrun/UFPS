using UnityEngine;

namespace AltSystems.AltBackup
{
    public class AltBackupIdTerrains : ScriptableObject
    {
        public int[] id_terrains;
        public TerrainData[] terrains;

        public int[] id_textures;
        public Texture2D[] textures;

        public int[] id_treesAndDetailMeshes;
        public GameObject[] treesAndDetailMeshes;

        private int i = -1;

        public int getIdTerrain(TerrainData td)
        {
            if (terrains == null || id_terrains == null)
            {
                terrains = new TerrainData[0];
                id_terrains = new int[0];
            }

            for (i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] != null && terrains[i].Equals(td))
                    return id_terrains[i];
            }

            return addTerrain(td);
        }

        public TerrainData getTerrain(int id)
        {
            if (terrains == null || id_terrains == null)
            {
                terrains = new TerrainData[0];
                id_terrains = new int[0];
            }

            for (i = 0; i < id_terrains.Length; i++)
            {
                if (id == id_terrains[i])
                    return terrains[i];
            }
            return null;
        }

        public int getIdTexture(Texture2D tx)
        {
            if (textures == null || id_textures == null)
            {
                textures = new Texture2D[0];
                id_textures = new int[0];
            }

            for (i = 0; i < textures.Length; i++)
            {
                if (textures[i] != null && textures[i].Equals(tx))
                    return id_textures[i];
            }

            return addTexture(tx);
        }

        public Texture2D getTexture(int id)
        {
            if (textures == null || id_textures == null)
            {
                textures = new Texture2D[0];
                id_textures = new int[0];
            }

            for (i = 0; i < id_textures.Length; i++)
            {
                if (id == id_textures[i])
                    return textures[i];
            }
            return null;
        }

        public void setTexture(int id, Texture2D tex)
        {
            bool isOk = false;
            if (textures == null || id_textures == null)
            {
                textures = new Texture2D[0];
                id_textures = new int[0];
            }

            for (i = 0; i < id_textures.Length; i++)
            {
                if (id == id_textures[i])
                {
                    isOk = true;
                    textures[i] = tex;
                }
            }

            if (!isOk)
            {
                int idTemp = addTextureNull(id);
                textures[idTemp] = tex;
            }
        }

        public int getIdTreesAndDetailMeshes(GameObject go)
        {
            if (treesAndDetailMeshes == null || id_treesAndDetailMeshes == null)
            {
                treesAndDetailMeshes = new GameObject[0];
                id_treesAndDetailMeshes = new int[0];
            }

            for (i = 0; i < treesAndDetailMeshes.Length; i++)
            {
                if (treesAndDetailMeshes[i] != null && treesAndDetailMeshes[i].Equals(go))
                    return id_treesAndDetailMeshes[i];
            }

            return addTreesAndDetailMeshes(go);
        }

        public GameObject getTreesAndDetailMeshes(int id)
        {
            if (treesAndDetailMeshes == null || id_treesAndDetailMeshes == null)
            {
                treesAndDetailMeshes = new GameObject[0];
                id_treesAndDetailMeshes = new int[0];
            }

            for (i = 0; i < id_treesAndDetailMeshes.Length; i++)
            {
                if (id == id_treesAndDetailMeshes[i])
                    return treesAndDetailMeshes[i];
            }
            return null;
        }

        public void setTreesAndDetailMeshes(int id, GameObject go)
        {
            bool isOk = false;
            if (treesAndDetailMeshes == null || id_treesAndDetailMeshes == null)
            {
                treesAndDetailMeshes = new GameObject[0];
                id_treesAndDetailMeshes = new int[0];
            }

            for (i = 0; i < id_treesAndDetailMeshes.Length; i++)
            {
                if (id == id_treesAndDetailMeshes[i])
                {
                    treesAndDetailMeshes[i] = go;
                    isOk = true;
                }
            }

            if (!isOk)
            {
                int idTemp = addTreesAndDetailMeshesNull(id);
                treesAndDetailMeshes[idTemp] = go;
            }
        }

        private int addTerrain(TerrainData td)
        {
            int[] id_terrainsTemp = new int[id_terrains.Length];
            TerrainData[] terrainsTemp = new TerrainData[terrains.Length];

            int num = -1;
            bool isNext = false;

            while (num == -1)
            {
                isNext = false;
                int numTemp = Random.Range(10000, 99999);
                for (i = 0; i < id_terrains.Length; i++)
                {
                    if (numTemp == id_terrains[i])
                    {
                        isNext = true;
                        break;
                    }
                }
                if (!isNext)
                    num = numTemp;
            }

            for (i = 0; i < terrains.Length; i++)
            {
                id_terrainsTemp[i] = id_terrains[i];
                terrainsTemp[i] = terrains[i];
            }

            id_terrains = new int[id_terrains.Length + 1];
            terrains = new TerrainData[terrains.Length + 1];

            for (i = 0; i < id_terrainsTemp.Length; i++)
            {
                id_terrains[i] = id_terrainsTemp[i];
                terrains[i] = terrainsTemp[i];
            }
            id_terrains[id_terrains.Length - 1] = num;
            terrains[terrains.Length - 1] = td;

            return num;
        }

        private int addTexture(Texture2D tx)
        {
            int[] id_texturesTemp = new int[id_textures.Length];
            Texture2D[] texturesTemp = new Texture2D[textures.Length];

            int num = -1;
            bool isNext = false;

            while (num == -1)
            {
                isNext = false;
                int numTemp = Random.Range(10000, 99999);
                for (i = 0; i < id_textures.Length; i++)
                {
                    if (numTemp == id_textures[i])
                    {
                        isNext = true;
                        break;
                    }
                }
                if (!isNext)
                    num = numTemp;
            }

            for (i = 0; i < textures.Length; i++)
            {
                id_texturesTemp[i] = id_textures[i];
                texturesTemp[i] = textures[i];
            }

            id_textures = new int[id_textures.Length + 1];
            textures = new Texture2D[textures.Length + 1];

            for (i = 0; i < id_texturesTemp.Length; i++)
            {
                id_textures[i] = id_texturesTemp[i];
                textures[i] = texturesTemp[i];
            }
            id_textures[id_textures.Length - 1] = num;
            textures[textures.Length - 1] = tx;

            return num;
        }

        private int addTextureNull(int num)
        {
            for (i = 0; i < id_textures.Length; i++)
            {
                if (num == id_textures[i])
                {
                    return i;
                }
            }


            int[] id_texturesTemp = new int[id_textures.Length];
            Texture2D[] texturesTemp = new Texture2D[textures.Length];


            for (i = 0; i < textures.Length; i++)
            {
                id_texturesTemp[i] = id_textures[i];
                texturesTemp[i] = textures[i];
            }

            id_textures = new int[id_textures.Length + 1];
            textures = new Texture2D[textures.Length + 1];

            for (i = 0; i < id_texturesTemp.Length; i++)
            {
                id_textures[i] = id_texturesTemp[i];
                textures[i] = texturesTemp[i];
            }
            id_textures[id_textures.Length - 1] = num;
            textures[textures.Length - 1] = null;

            return textures.Length - 1;
        }

        private int addTreesAndDetailMeshes(GameObject go)
        {
            int[] id_treesAndDetailMeshesTemp = new int[id_treesAndDetailMeshes.Length];
            GameObject[] treesAndDetailMeshesTemp = new GameObject[treesAndDetailMeshes.Length];

            int num = -1;
            bool isNext = false;

            while (num == -1)
            {
                isNext = false;
                int numTemp = Random.Range(10000, 99999);
                for (i = 0; i < id_treesAndDetailMeshes.Length; i++)
                {
                    if (numTemp == id_treesAndDetailMeshes[i])
                    {
                        isNext = true;
                        break;
                    }
                }
                if (!isNext)
                    num = numTemp;
            }

            for (i = 0; i < treesAndDetailMeshes.Length; i++)
            {
                id_treesAndDetailMeshesTemp[i] = id_treesAndDetailMeshes[i];
                treesAndDetailMeshesTemp[i] = treesAndDetailMeshes[i];
            }

            id_treesAndDetailMeshes = new int[id_treesAndDetailMeshes.Length + 1];
            treesAndDetailMeshes = new GameObject[treesAndDetailMeshes.Length + 1];

            for (i = 0; i < id_treesAndDetailMeshesTemp.Length; i++)
            {
                id_treesAndDetailMeshes[i] = id_treesAndDetailMeshesTemp[i];
                treesAndDetailMeshes[i] = treesAndDetailMeshesTemp[i];
            }
            id_treesAndDetailMeshes[id_treesAndDetailMeshes.Length - 1] = num;
            treesAndDetailMeshes[treesAndDetailMeshes.Length - 1] = go;

            return num;
        }

        private int addTreesAndDetailMeshesNull(int num)
        {
            for (i = 0; i < id_treesAndDetailMeshes.Length; i++)
            {
                if (num == id_treesAndDetailMeshes[i])
                {
                    return i;
                }
            }

            int[] id_treesAndDetailMeshesTemp = new int[id_treesAndDetailMeshes.Length];
            GameObject[] treesAndDetailMeshesTemp = new GameObject[treesAndDetailMeshes.Length];

            for (i = 0; i < treesAndDetailMeshes.Length; i++)
            {
                id_treesAndDetailMeshesTemp[i] = id_treesAndDetailMeshes[i];
                treesAndDetailMeshesTemp[i] = treesAndDetailMeshes[i];
            }

            id_treesAndDetailMeshes = new int[id_treesAndDetailMeshes.Length + 1];
            treesAndDetailMeshes = new GameObject[treesAndDetailMeshes.Length + 1];

            for (i = 0; i < id_treesAndDetailMeshesTemp.Length; i++)
            {
                id_treesAndDetailMeshes[i] = id_treesAndDetailMeshesTemp[i];
                treesAndDetailMeshes[i] = treesAndDetailMeshesTemp[i];
            }

            id_treesAndDetailMeshes[id_treesAndDetailMeshes.Length - 1] = num;
            treesAndDetailMeshes[treesAndDetailMeshes.Length - 1] = null;

            return treesAndDetailMeshes.Length - 1;
        }
    }
}

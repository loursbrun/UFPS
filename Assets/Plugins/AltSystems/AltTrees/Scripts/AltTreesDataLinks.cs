using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace AltSystems.AltTrees
{
    public class AltTreesDataLinks : ScriptableObject
    {
	    [SerializeField]
	    public int[] id;
	    [SerializeField]
	    public GameObject[] trees;
	    [SerializeField]
	    public AltTree[] altTrees;

        public bool checkTreeVersionsStatus()
        {
            if (altTrees == null)
                return false;

            #if UNITY_EDITOR
            {

                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null)
                        if (altTrees[i].checkVersionTreeStatus())
                            return true;
                }
            }
            #endif
            return false;
        }

        public AltTree[] checkTreeVersions()
        {
            if (altTrees == null)
                return null;

            #if UNITY_EDITOR
            {
                int countTrees = 0;
                int countTrees2 = 0;

                for (int i = 0; i < altTrees.Length; i++)
                {
                    if (altTrees[i] != null)
                        if (altTrees[i].checkVersionTreeStatus())
                            countTrees++;
                }
                if (countTrees > 0)
                {
                    AltTree[] altTT = new AltTree[countTrees];

                    for (int i = 0; i < altTrees.Length; i++)
                    {
                        if (altTrees[i] != null)
                        {
                            if (altTrees[i].checkVersionTreeStatus())
                            {
                                altTT[countTrees2] = altTrees[i];
                                countTrees2++;
                            }
                        }
                    }

                    return altTT;
                }
                else
                    return null;
            }
            #else
                return null;
            #endif
        }


        public int getId(GameObject tree)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<trees.Length;i++)
			    {
				    if(trees[i] != null && trees[i].Equals(tree))
					    return id[i];
			    }

			    return -1;
		    }
		    else 
			    return -1;
	    }
	    public int getId(AltTree tree)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<altTrees.Length;i++)
			    {
				    if(altTrees[i] != null && altTrees[i].Equals(tree))
					    return id[i];
			    }
			
			    return -1;
		    }
		    else 
			    return -1;
	    }


	    public GameObject getTree(int _id)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<id.Length;i++)
			    {
				    if(id[i]== _id && trees[i] != null)
					    return trees[i];
			    }
			
			    return null;
		    }
		    else
			    return null;
	    }
	    public GameObject getTree(AltTree tree)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<altTrees.Length;i++)
			    {
				    if(altTrees[i] != null && altTrees[i].Equals(tree) && trees[i] != null)
					    return trees[i];
			    }
			
			    return null;
		    }
		    else
			    return null;
	    }
	


	    public AltTree getAltTree(int _id)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<id.Length;i++)
			    {
				    if(id[i]== _id && altTrees[i] != null)
					    return altTrees[i];
			    }
			
			    return null;
		    }
		    else
			    return null;
	    }
	    public AltTree getAltTree(GameObject tree)
	    {
		    if(checkVars())
		    {
			    for(int i=0;i<trees.Length;i++)
			    {
                    if (trees[i] != null && trees[i].Equals(tree) && altTrees[i] != null)
					    return altTrees[i];
			    }
			
			    return null;
		    }
		    else
			    return null;
	    }


	    public int addTree(GameObject tree, AltTree altTree, int _id)
	    {
		    if(checkVarsAdd())
            {
                for (int i = 0; i < id.Length; i++)
                {
                    if(id[i] == _id && altTrees[i] == null)
                    {
                        trees[i] = null;
                        altTrees[i] = altTree;
                        return _id;
                    }
                }


                int[] idTemp = id;
			    GameObject[] treesTemp = trees;
			    AltTree[] altTreesTemp = altTrees;


			    id = new int[id.Length+1];
			    trees = new GameObject[trees.Length+1];
			    altTrees = new AltTree[altTrees.Length+1];
			
			    for(int i=0;i<idTemp.Length;i++)
			    {
				    id[i] = idTemp[i];
				    trees[i] = treesTemp[i];
				    altTrees[i] = altTreesTemp[i];
			    }
                id[id.Length - 1] = _id;
			    trees[trees.Length-1] = tree;
			    altTrees[trees.Length-1] = altTree;
                altTree.id = _id;

                return _id;
		    }
		    else 
			    return -1;
	    }
	
	    bool checkVars()
	    {
		    if(id==null || trees==null || altTrees==null)
		    {
			    return false;
		    }
		    else
		    {
			    if(id.Length != trees.Length || id.Length != altTrees.Length)
			    {
				    Debug.LogError("Error! id.Length != trees.Length || id.Length != altTrees.Length");
				    return false;
			    }
			    else
				    return true;
		    }
	    }
	    bool checkVarsAdd()
	    {
		    if(id==null || trees==null || altTrees==null)
		    {
			    id = new int[0];
			    trees = new GameObject[0];
			    altTrees = new AltTree[0];

			    return true;
		    }
		    else
		    {
			    if(id.Length != trees.Length || id.Length != altTrees.Length)
			    {
				    Debug.LogError("Error! id.Length != trees.Length || id.Length != altTrees.Length");
				    return false;
			    }
			    else
				    return true;
		    }
	    }

        public int getUniqueIdTree()
        {
            int num = -1;
            bool isNext = false;

            if (id == null || id.Length == 0)
            {
                num = Random.Range(1000000, 9999999);
            }
            else
            {
                while (num == -1)
                {
                    isNext = false;
                    int numTemp = Random.Range(1000000, 9999999);
                    for (int i = 0; i < id.Length; i++)
                    {
                        if (numTemp == id[i])
                        {
                            isNext = true;
                            break;
                        }
                    }
                    if (!isNext)
                        num = numTemp;
                }
            }
            return num;
        }

    }
}


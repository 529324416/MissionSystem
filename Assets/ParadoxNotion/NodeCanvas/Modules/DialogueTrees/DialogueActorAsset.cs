using UnityEngine;


namespace NodeCanvas.DialogueTrees
{

    ///<summary> A DialogueActor Asset.</summary>
    [CreateAssetMenu(menuName = "ParadoxNotion/NodeCanvas/Dialogue Actor")]
    public class DialogueActorAsset : ScriptableObject, IDialogueActor
    {
        [SerializeField] protected string _name;
        [SerializeField] protected Texture2D _portrait;
        [SerializeField] protected Color _dialogueColor = Color.white;
        [SerializeField] protected Vector3 _dialogueOffset;

        private Sprite _portraitSprite;

        new public string name => _name;
        public Texture2D portrait => _portrait;
        public Color dialogueColor => _dialogueColor;
        public Vector3 dialoguePosition => Vector3.zero;
        public Transform transform => null;
        public Sprite portraitSprite {
            get
            {
                if ( _portraitSprite == null && portrait != null )
                    _portraitSprite = Sprite.Create(portrait, new Rect(0, 0, portrait.width, portrait.height), new Vector2(0.5f, 0.5f));
                return _portraitSprite;
            }
        }
    }
}

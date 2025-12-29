namespace lobby.recruit
{
    using UnityEngine;
    using System.Collections.Generic;
    using persistence;

    public class RecruitPartyUI : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private Transform listRoot;
        [SerializeField] private GameObject partyMemberPrefab;

        private bool _hasStarted = false;

        private void Start()
        {
            _hasStarted = true; 
            
            if (RunManager.Instance != null)
            {
                RunManager.Instance.OnPartyChanged += RefreshUI;
                RefreshUI();
            }
        }

        private void OnEnable()
        {
            if (_hasStarted)
            {
                RefreshUI();
            }
        }

        private void OnDestroy()
        {
            if (RunManager.Instance != null)
            {
                RunManager.Instance.OnPartyChanged -= RefreshUI;
            }    
        }
        
        public void RefreshUI()
        {
            if (RunManager.Instance == null) return;

            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var hero in RunManager.Instance.party)
            {
                GameObject go = Instantiate(partyMemberPrefab, listRoot);
                
                var draggable = go.GetComponent<PartyListDraggable>();
                if (draggable != null)
                {
                    draggable.Init(hero, this);
                }
                else
                {
                    Debug.LogError("Party member prefab is missing PartyListDraggable component!");
                }
            }
        }

        public void OnListReordered()
        {
            List<HeroData> newOrder = new List<HeroData>();
            
            foreach (Transform child in listRoot)
            {
                var draggable = child.GetComponent<PartyListDraggable>();
                if (draggable != null)
                {
                    newOrder.Add(draggable.Data);
                }
            }

            RunManager.Instance.UpdatePartyList(newOrder);
            
            Debug.Log($"Party reordered. New order count: {newOrder.Count}");
        }
    }
}
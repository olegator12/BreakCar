#if UNITY_EDITOR
using Economic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ProgressEditor : MonoBehaviour
    {
        [SerializeField] private GameObject _holder;
        [SerializeField] private Button _add;
        [SerializeField] private Button _clear;

        private Wallet _wallet;
        private CardHolder _cardHolder;

        private void OnDestroy()
        {
            _add.onClick.RemoveListener(OnClickAdd);
            _clear.onClick.RemoveListener(OnClearAdd);
        }

        public void Initialize(Wallet wallet, CardHolder cardHolder)
        {
            _wallet = wallet;
            _cardHolder = cardHolder;
            _holder.SetActive(true);

            _add.onClick.AddListener(OnClickAdd);
            _clear.onClick.AddListener(OnClearAdd);
        }

        private void OnClickAdd()
        {
            _wallet.AddMoney(1000);
            _wallet.ApplyModification();
            _cardHolder.Draw();
        }

        private void OnClearAdd()
        {
            _wallet.SpendMoney(_wallet.MoneyCount);
            _wallet.ApplyModification();
            _cardHolder.Draw();
        }
    }
}
#endif
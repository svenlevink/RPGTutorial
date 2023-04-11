using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryUIState { ItemSelection, PartySelection, Busy, MoveToForget }

public class InventoryUI : MonoBehaviour
{
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;

    [SerializeField] Text categoryText;
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;

    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;
    [SerializeField] Image leftArrow;
    [SerializeField] Image rightArrow;

    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelectionUI;

    Action<ItemBase> onItemUsed;

    int selectedItem = 0;
    int selectedCategory = 0;
    MoveBase moveToLearn;

    InventoryUIState state;

    List<ItemSlotUI> slotUIList;

    Inventory inventory;

    //de grootte van de item box
    RectTransform itemListRect;

    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();

        inventory.OnUpdated += UpdateItemList;
    }

    void UpdateItemList()
    {
        //clear all existing Items (Kill all the children)
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        //maak een Itemslot een child in ItemSlotUi. Dus je maakt hier de slots voor de items aan. voor alle items die je aan player toevoegt
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slotUIObj = Instantiate(itemSlotUI, itemList.transform);
            slotUIObj.SetData(itemSlot);

            slotUIList.Add(slotUIObj);
        }

        UpdateItemSelection();
    }

    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed=null)
    {
        this.onItemUsed = onItemUsed;

        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;

            if (Input.GetKeyDown(KeyCode.DownArrow))
                ++selectedItem;
            else if (Input.GetKeyDown(KeyCode.UpArrow))
                --selectedItem;
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                ++selectedCategory;
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                --selectedCategory;

            //rotates the selection of categories.
            if (selectedCategory > Inventory.ItemCategories.Count - 1)
                selectedCategory = 0;
            else if (selectedCategory < 0)
                selectedCategory = Inventory.ItemCategories.Count - 1;


            selectedItem = Mathf.Clamp(selectedItem, 0, inventory.GetSlotsByCategory(selectedCategory).Count - 1);

            //begint d elijst weer bij 0 en verandert de naam van de categorie in de UI
            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
            {
                UpdateItemSelection();
            }

            if (Input.GetKeyDown(KeyCode.Space))
                StartCoroutine(ItemSelected());
            else if (Input.GetKeyDown(KeyCode.LeftShift))
                onBack?.Invoke();
        }
        else if (state == InventoryUIState.PartySelection)
        {
            //handle party selection
            Action onSelected = () =>
            {
                StartCoroutine(UseItem());
            };

            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };

            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {
            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(OnMoveToForgetSelected(moveIndex));
            };

            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }

    IEnumerator ItemSelected()
    {
        //will block everything wile routine is running
        state = InventoryUIState.Busy;

        var item = inventory.GetItem(selectedItem, selectedCategory);

        if (GameController.Instance.State == GameState.Battle)
        {
            //in Battle
            if (!item.CanUseInBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"There is a time and place for everything, but this ain't it.");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            //outside battle
            if (!item.CanUseOutsideBattle)
            {
                yield return DialogManager.Instance.ShowDialogText($"There is a time and place for everything, but this ain't it.");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }

        //check if category is pokeball
        if (selectedCategory == (int)ItemCategory.Pokeballs)
        {
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TmItem)
                //show TM is usable
                partyScreen.ShowIfTmIsUsable(item as TmItem);
        }
    }

    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTmItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.Instance.ShowDialogText($"You used {usedItem.Name}");

            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (selectedCategory == (int)ItemCategory.Items)
                yield return DialogManager.Instance.ShowDialogText($"It won't have any effect.");
        }

        ClosePartyScreen();
    }

    //TM & HM function
    IEnumerator HandleTmItems()
    {
        //cast as a TMItem. Will return null if item is not tmitem
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TmItem;
        if (tmItem == null)
            yield break;

        //zo kan je in deze file pokemon shit achterhalen!!!!!! (de eertse lijn)
        var pokemon = partyScreen.SelectedMember;

        if (pokemon.HasMove(tmItem.Move))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} already knows {tmItem.Move.Name}.");
            yield break;
        }

        //kijk of de tm move in de lijst van de pokemon staat
        if (!tmItem.CanBeTaught(pokemon))
        {
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} can't learn {tmItem.Move.Name}.");
            yield break;
        }

        if (pokemon.Moves.Count < 4)
        {
            pokemon.LearnMove(tmItem.Move);
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} learned {tmItem.Move.Name}!");
        }
        else
        {
            //forget move
            //dialogmanager is used outside of battle
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} wants to learn the move {tmItem.Move.Name}.");
            yield return DialogManager.Instance.ShowDialogText($"But it already knows 4 moves.");
            yield return ChooseMoveToForget(pokemon, tmItem.Move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
            yield return new WaitForSeconds(2f);
        }
    }

    IEnumerator ChooseMoveToForget(Pokemon pokemon, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.Instance.ShowDialogText($"Choose a move to forget", true, false);
        moveSelectionUI.gameObject.SetActive(true);
        //zet de moves neer die de pokemon al kende plus de te leren move. Dus 5 moves in totaal
        moveSelectionUI.SetMoveData(pokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveToLearn = newMove;

        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
       var slots = inventory.GetSlotsByCategory(selectedCategory);

        //Zorgen dat de index van de lijst niet focked op is na gebruiken laatste item.
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

        for (int i = 0; i < slotUIList.Count; i++)
        {
            if (i == selectedItem)
                slotUIList[i].NameText.color = GlobalSettings.i.HighlightedColor;
            else
                slotUIList[i].NameText.color = Color.black;
        }
      
        if (slots.Count > 0)
        {
            var item = slots[selectedItem].Item;
            itemIcon.sprite = item.Icon;
            itemDescription.text = item.Description;
        }
        
        HandleScrolling();
    }

    void HandleScrolling()
    {
        if (slotUIList.Count <= 4) return;
        //scroll begint na 5de item
        float scrollPos = Mathf.Clamp(selectedItem - 4, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        //pijl pas laten zien na 5 items omlaag
        bool showUpArrow = selectedItem > 4;
        upArrow.gameObject.SetActive(showUpArrow);

        //pijl niet meer laten zien als je bijna bij het laatste item bent.
        bool showDownArrow = selectedItem + 4 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    //zet alles ff uit nadat je naar de volgende categorie gaat
    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);

        itemIcon.sprite = null;
        itemDescription.text = "";
    }

    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);
    }

    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;

        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }

    IEnumerator OnMoveToForgetSelected(int moveIndex)
    {
        var pokemon = partyScreen.SelectedMember;

        DialogManager.Instance.CloseDialog();
        moveSelectionUI.gameObject.SetActive(false);
        if (moveIndex == 4)
        {
            //Pokemon will not learn new move
            //also de dialog wacht totdat de speler op spatie drukt
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} did not learn {moveToLearn.Name}.");
        }
        else
        {
            //pokemon should forget old move and learn new one
            var selectedMove = pokemon.Moves[moveIndex].Base;
            yield return DialogManager.Instance.ShowDialogText($"{pokemon.Base.Name} forgot {selectedMove.Name} and learned {moveToLearn.Name}.");

            //replaces the old move with the new one
            pokemon.Moves[moveIndex] = new Move(moveToLearn);
        }
        moveToLearn = null;
        //en door!
        state = InventoryUIState.ItemSelection;
    }
}

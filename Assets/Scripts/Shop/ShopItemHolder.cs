using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ShopItemHolder : MonoBehaviour
{
    [SerializeField] private ShopItem item;
    [SerializeField] private string title;
    [SerializeField] private string description;
    [SerializeField] private int cost;

    [Space] [Header("UI Elements")]
    [SerializeField] private TMP_Text titleText;

    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradeLevelNumber;
    [SerializeField] private SpriteFromAtlas shopItemPicture;
    [SerializeField] private SpriteFromAtlas shopItemTemplate;
    [SerializeField] private GameObject checkMark;

    private static readonly List<ShopItemHolder> ShopItemHolders = new();
    private Button buyButton;

    public static void RefreshShopUI()
    {
        ShopItemHolders.ForEach(holder => holder.RefreshUI());
    }
    
    public void Awake()
    {
        ShopItemHolders.Add(this);
    } 
    public void Start()
    {
        buyButton = GetComponentInChildren<Button>();
        
        InstantiateShopItem();
        if (item is BoatSkin)
        {
            UpgradeHolder.Instance.SelectIfCurrentSkin(this);
        }
    }

    private void InstantiateShopItem()
    {
        if (item == null)
        {
            return;
        }
        title = item.Title;
        description = item.Description;
        cost = item.Cost;
        RefreshUI();
    }

    private void AddShopItem()
    {
        if (item is Upgrade upgrade)
        {
            UpgradeHolder.Instance.AddUpgrade(upgrade.UpgradeType);
        }
        else if (item is BoatSkin)
        {
            UpgradeHolder.Instance.ApplyBoatShopItem(this);
        }
    }
    private bool CanPress()
    {
        bool canAfford = cost <= GameManager.Instance.Coins;
        bool isBelowMaxLevel = item is not Upgrade upgrade || UpgradeHolder.Instance.GetUpgradeLevel(upgrade.UpgradeType) < Upgrade.MaxLevel;
        return (canAfford && isBelowMaxLevel) || ItemIsOwnedSkin();
    }
    private bool ItemIsOwnedSkin()
    {
        return item is BoatSkin skin && UpgradeHolder.Instance.HasBoatSkin(skin);
    }
    public ShopItem GetItem()
    {
        return item;
    }
    public void BuyItem()
    {
        if (!CanPress())
        {
            return;
        }
        if (ItemIsOwnedSkin())
        {
            UpgradeHolder.Instance.ApplyBoatShopItem(this);
            return;
        }
        GameManager.Instance.Coins -= cost;
        AddShopItem();
    }

    private void RefreshUI()
    {
        if (item == null) return;
        shopItemPicture.ChangeSprite(item.ItemPicture.sprite.name);
        shopItemTemplate.ChangeSprite(item.ItemTemplate.sprite.name);
        titleText.text = title;
        descriptionText.text = description;
        costText.text = cost.ToString();
        
        buyButton.interactable = CanPress();
        if (item is Upgrade upgrade)
        {
            upgradeLevelNumber.text = UpgradeHolder.Instance.GetUpgradeLevel(upgrade.UpgradeType).ToString();
        }
        else
        {
            BoatSkin boatSkin = item as BoatSkin;
            upgradeLevelNumber.text = UpgradeHolder.Instance.HasBoatSkin(boatSkin) ? "Owned" : "";
        }
    }

    public void ActivateCheckMark()
    {
        checkMark.SetActive(true);
    }
    
    public void DisableCheckMark()
    {
        checkMark.SetActive(false);
    }
}

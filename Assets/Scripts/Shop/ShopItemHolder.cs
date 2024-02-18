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
    [SerializeField] private bool isBought;

    [Space] [Header("UI Elements")] [SerializeField]
    private TMP_Text titleText;

    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text upgradeLevelNumber;
    [SerializeField] private SpriteFromAtlas shopItemPicture;
    [SerializeField] private SpriteFromAtlas shopItemTemplate;
    [SerializeField] private GameObject checkMark;
    private Button buyButton;

    public void Start()
    {
        buyButton = GetComponentInChildren<Button>();
        if (item != null)
        {
            InstantiateShopItem(item);
            if (item is BoatSkin)
            {
                UpgradeHolder.Instance.SelectIfCurrentSkin(this);
            }
        }
    }

    public void InstantiateShopItem(ShopItem newItem)
    {
        item = newItem;
        title = item.Title;
        description = item.Description;
        cost = item.Cost;
        RefreshUI();
    }

    private void AddShopItem()
    {
        UpgradeHolder holder = UpgradeHolder.Instance;
        if (isUpgrade() != null)
        {
            Upgrade upgrade = item as Upgrade;
            holder.AddUpgrade(upgrade.UpgradeType);
        }
        else if (isBoatSkin())
        {
            holder.ApplyBoatShopItem(this);
        }
    }

    private Upgrade isUpgrade()
    {
        if (item is Upgrade)
        {
            return item as Upgrade;
        }

        return null;
    }

    private BoatSkin isBoatSkin()
    {
        if (item is BoatSkin)
        {
            return item as BoatSkin;
        }

        return null;
    }

    private bool isRightlevel(Upgrade upgrade)
    {
        if (UpgradeHolder.Instance.GetUpgradeLevel(upgrade.UpgradeType) >= 3)
        {
            return false;
        }

        return true;
    }

    public bool CanBuy()
    {
        if (GameManager.Instance.Coins >= cost)
        {
            return true;
        }

        return false;
    }

    public ShopItem GetItem()
    {
        return item;
    }

    public void BuyItem()
    {
        if (CanBuy())
        {
            if (isUpgrade() != null && isRightlevel(item as Upgrade) == false)
            {
                return;
            }

            if (isBoatSkin() != null && UpgradeHolder.Instance.HasBoatSkin(item as BoatSkin))
            {
                UpgradeHolder.Instance.ApplyBoatShopItem(this);
                return;
            }
            GameManager.Instance.Coins -= cost;
            AddShopItem();
            RefreshUI();
        }
    }

    public void RefreshUI()
    {
        if (item == null) return;
        shopItemPicture.ChangeSprite(item.ItemPicture.sprite.name);
        shopItemTemplate.ChangeSprite(item.ItemTemplate.sprite.name);
        titleText.text = title;
        descriptionText.text = description;
        costText.text = cost.ToString();
        
        if (item is Upgrade)
        {
            Upgrade upgrade = item as Upgrade;
            if (CanBuy() && isRightlevel(item as Upgrade) == false)
            {
                buyButton.interactable = false;
            }
            else
            {
                buyButton.interactable = true;
            }
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

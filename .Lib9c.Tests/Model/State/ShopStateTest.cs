namespace Lib9c.Tests.Model.State
{
    using System;
    using Bencodex.Types;
    using Libplanet;
    using Libplanet.Assets;
    using Libplanet.Crypto;
    using Nekoyume.Model.Item;
    using Nekoyume.Model.State;
    using Nekoyume.TableData;
    using Xunit;

    public class ShopStateTest
    {
        [Fact]
        public void Serialization()
        {
            var shopState = new ShopState();
            var agentAddress = new PrivateKey().ToAddress();
            var avatarAddress = new PrivateKey().ToAddress();
            var productId = Guid.NewGuid();
            var weaponRow = new EquipmentItemSheet.Row();
            weaponRow.Set(new[]
            {
                "10100000", "Weapon", "0", "Normal", "0", "ATK", "1", "2", "10100000",
            });
            var itemUsable = new Weapon(
                weaponRow,
                Guid.NewGuid(),
                0);
            var price = new FungibleAssetValue(new Currency("NCG", 2, minter: null));
            var shopItem = new ShopItem(
                agentAddress,
                avatarAddress,
                productId,
                itemUsable,
                price
            );

            shopState.Register(shopItem);
            var serialized = (Dictionary)shopState.Serialize();
            shopState = new ShopState(serialized);

            Assert.Equal(1, shopState.Products.Count);
            Assert.Contains(productId, shopState.Products);
            Assert.Equal(shopItem, shopState.Products[productId]);
        }

        [Fact]
        public void Register()
        {
            var shopState = new ShopState();
            var agentAddress = new PrivateKey().ToAddress();
            var avatarAddress = new PrivateKey().ToAddress();
            var productId = Guid.NewGuid();
            var weaponRow = new EquipmentItemSheet.Row();
            weaponRow.Set(new[]
            {
                "10100000", "Weapon", "0", "Normal", "0", "ATK", "1", "2", "10100000",
            });
            var itemUsable = new Weapon(
                weaponRow,
                Guid.NewGuid(),
                0);
            var price = new FungibleAssetValue(new Currency("NCG", 2, minter: null));
            var shopItem = new ShopItem(
                agentAddress,
                avatarAddress,
                productId,
                itemUsable,
                price
            );

            shopState.Register(shopItem);

            Assert.Equal(1, shopState.Products.Count);
            Assert.Contains(productId, shopState.Products);
            Assert.Equal(shopItem, shopState.Products[productId]);
        }

        [Fact]
        public void Unregister()
        {
            var shopState = new ShopState();
            var agentAddress = new PrivateKey().ToAddress();
            var avatarAddress = new PrivateKey().ToAddress();
            var productId = Guid.NewGuid();
            var weaponRow = new EquipmentItemSheet.Row();
            weaponRow.Set(new[]
            {
                "10100000", "Weapon", "0", "Normal", "0", "ATK", "1", "2", "10100000",
            });
            var itemUsable = new Weapon(
                weaponRow,
                Guid.NewGuid(),
                0);
            var price = new FungibleAssetValue(new Currency("NCG", 2, minter: null));
            var shopItem = new ShopItem(
                agentAddress,
                avatarAddress,
                productId,
                itemUsable,
                price
            );

            shopState.Register(shopItem);
            shopState.Unregister(shopItem);

            Assert.Equal(0, shopState.Products.Count);

            Assert.Throws<FailedToUnregisterInShopStateException>(() =>
                shopState.Unregister(shopItem));
        }

        [Fact]
        public void TryUnregister()
        {
            var shopState = new ShopState();
            var agentAddress = new PrivateKey().ToAddress();
            var avatarAddress = new PrivateKey().ToAddress();
            var productId = Guid.NewGuid();
            var weaponRow = new EquipmentItemSheet.Row();
            weaponRow.Set(new[]
            {
                "10100000", "Weapon", "0", "Normal", "0", "ATK", "1", "2", "10100000",
            });
            var itemUsable = new Weapon(
                weaponRow,
                Guid.NewGuid(),
                0);
            var price = new FungibleAssetValue(new Currency("NCG", 2, minter: null));
            var shopItem = new ShopItem(
                agentAddress,
                avatarAddress,
                productId,
                itemUsable,
                price
            );

            shopState.Register(shopItem);

            Assert.True(shopState.TryUnregister(
                shopItem.ProductId,
                out var unregisteredItem));
            Assert.Equal(shopItem, unregisteredItem);

            Assert.Equal(0, shopState.Products.Count);

            Assert.Throws<FailedToUnregisterInShopStateException>(() =>
                shopState.Unregister(shopItem));
        }

        [Fact]
        public void TryGet()
        {
            var shopState = new ShopState();
            var agentAddress = new PrivateKey().ToAddress();
            var avatarAddress = new PrivateKey().ToAddress();
            var productId = Guid.NewGuid();
            var weaponRow = new EquipmentItemSheet.Row();
            weaponRow.Set(new[]
            {
                "10100000", "Weapon", "0", "Normal", "0", "ATK", "1", "2", "10100000",
            });
            var itemUsable = new Weapon(
                weaponRow,
                Guid.NewGuid(),
                0);
            var price = new FungibleAssetValue(new Currency("NCG", 2, minter: null));
            var shopItem = new ShopItem(
                agentAddress,
                avatarAddress,
                productId,
                itemUsable,
                price
            );

            shopState.Register(shopItem);

            Assert.True(shopState.TryGet(agentAddress, shopItem.ProductId, out var outShopItem));
            Assert.Equal(shopItem, outShopItem);

            shopState.Unregister(shopItem);

            Assert.False(shopState.TryGet(agentAddress, shopItem.ProductId, out _));
        }
    }
}

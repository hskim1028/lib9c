namespace Lib9c.Tests.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Lib9c.Tests.Action;
    using Libplanet.Action;
    using Nekoyume.Battle;
    using Nekoyume.Model;
    using Nekoyume.Model.BattleStatus;
    using Nekoyume.Model.Skill;
    using Nekoyume.Model.State;
    using Priority_Queue;
    using Xunit;

    public class PlayerTest
    {
        private readonly IRandom _random;
        private readonly AvatarState _avatarState;
        private readonly TableSheets _tableSheets;

        public PlayerTest()
        {
            _random = new ItemEnhancementTest.TestRandom();
            _tableSheets = new TableSheets(TableSheetsImporter.ImportSheets());
            _avatarState = new AvatarState(
                default,
                default,
                0,
                _tableSheets.GetAvatarSheets(),
                new GameConfigState(),
                default
            );
        }

        [Fact]
        public void TickAlive()
        {
            var simulator = new StageSimulator(
                _random,
                _avatarState,
                new List<Guid>(),
                1,
                1,
                _tableSheets.GetStageSimulatorSheets()
            );
            var player = simulator.Player;
            var enemy = new Enemy(player, _tableSheets.CharacterSheet.Values.First(), 1);
            player.Targets.Add(enemy);
            player.InitAI();
            player.Tick();

            Assert.NotEmpty(simulator.Log);
            Assert.Equal(nameof(WaveTurnEnd), simulator.Log.Last().GetType().Name);
        }

        [Fact]
        public void TickDead()
        {
            var simulator = new StageSimulator(
                _random,
                _avatarState,
                new List<Guid>(),
                1,
                1,
                _tableSheets.GetStageSimulatorSheets()
            );
            var player = simulator.Player;
            var enemy = new Enemy(player, _tableSheets.CharacterSheet.Values.First(), 1);
            player.Targets.Add(enemy);
            player.InitAI();
            player.CurrentHP = -1;

            Assert.True(player.IsDead);

            player.Tick();

            Assert.NotEmpty(simulator.Log);
            Assert.Equal(nameof(WaveTurnEnd), simulator.Log.Last().GetType().Name);
        }

        [Theory]
        [InlineData(SkillCategory.DoubleAttack)]
        [InlineData(SkillCategory.AreaAttack)]
        public void UseDoubleAttack(SkillCategory skillCategory)
        {
            var skill = SkillFactory.Get(
                _tableSheets.SkillSheet.Values.First(r => r.SkillCategory == skillCategory),
                100,
                100
            );
            var simulator = new StageSimulator(
                _random,
                _avatarState,
                new List<Guid>(),
                1,
                1,
                _tableSheets.GetStageSimulatorSheets()
            );
            var player = simulator.Player;

            var enemy = new Enemy(player, _tableSheets.CharacterSheet.Values.First(), 1)
            {
                CurrentHP = 1,
            };
            player.Targets.Add(enemy);
            simulator.Characters = new SimplePriorityQueue<CharacterBase, decimal>();
            simulator.Characters.Enqueue(enemy, 0);
            player.InitAI();
            player.OverrideSkill(skill);
            Assert.Single(player.Skills);

            player.Tick();

            Assert.Single(simulator.Log.OfType<Dead>());
        }
    }
}

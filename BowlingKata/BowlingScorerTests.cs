using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace BowlingKata
{
	[TestFixture]
	public class BowlingScorerTests
	{
		[Test]
		[TestCase("00,00,00,00,00,00,00,00,00,00", 0)]
		[TestCase("10,00,00,00,00,00,00,00,00,00", 1)]
		[TestCase("10,10,10,10,10,10,10,10,10,10", 10)]
		[TestCase("11,11,11,11,11,11,11,11,11,11", 20)]
		[TestCase("73,00,32,31,33,91,53,23,52,33", 61)]
		[TestCase("7/,00,32,31,33,91,53,23,52,33", 61)]
		[TestCase("2/,11,00,00,00,00,00,00,00,00", 13)]
		[TestCase("x,00,32,31,33,91,53,23,52,33", 61)]
		[TestCase("x,11,00,00,00,00,00,00,00,00", 14)]
		[TestCase("x,x,x,00,00,00,00,00,00,00", 60)]
		public void Should_score_game_of_gutterballs(string game, int expected)
		{
			var score = new BowlingGame(game).GetScore();
			Assert.That(score, Is.EqualTo(expected));
		}// strike: bonus = previous 2 frames, spare: bonus = previous frame

		[Test]
		public void Should_score_perfect_game()
		{
			var score = new BowlingGame("x,x,x,x,x,x,x,x,x,x").GetScore();
			Assert.That(score, Is.EqualTo(300));
		}

	}

	public class BowlingGame
	{
		private readonly string _game;

		public BowlingGame(string game)
		{
			_game = game;
		}

		public int GetScore()
		{
			var throws = _game.Split(',').Select(t => new Frame(t));
			return ScoreThrows(throws.ToArray());
		}

		private static int ScoreThrows(IList<Frame> throws)
		{
			var total = throws.First().Score;

			for (var i = 1; i < throws.Count; i++)
			{
				var currentFrame = throws[i];
				var bonusScore = GetBonusScore(currentFrame, throws[i - 1]);
				total += currentFrame.Score + bonusScore;
			}
			return total;
		}

		private static int GetBonusScore(Frame currentFrame, Frame previousFrame)
		{
			if (previousFrame.FrameType == FrameType.Spare)
			{
				return currentFrame.Throw1;
			}
			if (previousFrame.FrameType == FrameType.Strike)
			{
				return currentFrame.Score;
			}

			return 0;
		}
	}

	class Frame
	{
		public FrameType FrameType { get; set; }

		public int Throw1 { get; set; }
		public int Throw2 { get; set; }

		public int Score
		{
			get { return Throw1 + Throw2; }
		}

		public Frame(string frame)
		{
			if (frame[0] == 'x')
			{
				FrameType = FrameType.Strike;
				Throw1 = 10;
			}
			else
			{
				Throw1 = int.Parse(frame[0].ToString());

				if (frame[1] == '/')
				{
					FrameType = FrameType.Spare;
					Throw2 = 10 - Throw1;
				}
				else
				{
					Throw2 = int.Parse(frame[1].ToString());
				}
			}
		}
	}

	enum FrameType
	{
		Normal,
		Spare,
		Strike
	}
}
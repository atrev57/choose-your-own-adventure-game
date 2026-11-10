using System;
using System.Collections.Generic;

// Test helper: a Die that returns a queued sequence of rolls instead of random ones.
public class ScriptedDie : Die
{
    private readonly Queue<int> _rolls;

    public ScriptedDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public override int Roll(int sides)
    {
        if (_rolls.Count == 0)
            throw new InvalidOperationException(
                $"ScriptedDie ran out of scripted rolls (asked for a d{sides}).");

        return _rolls.Dequeue();
    }
}

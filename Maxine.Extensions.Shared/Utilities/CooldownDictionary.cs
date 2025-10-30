using System.Collections.Concurrent;

namespace Poki.Services;

public class CooldownDictionary<TKey> where TKey : notnull
{
    private readonly TimeSpan _cooldown;
    private readonly ConcurrentDictionary<TKey, DateTime> _dict = new();

    private class Record
    {
        public readonly TimeSpan Cooldown;
        public bool PassesCooldown = true;
        
        public Record(TimeSpan cooldown)
        {
            Cooldown = cooldown;
        }
    }

    public CooldownDictionary(TimeSpan cooldown)
    {
        _cooldown = cooldown;
    }

    public bool CheckOrUpdateCooldown(TKey key)
    {
        var arg = new Record(_cooldown);

        _dict.AddOrUpdate(
            key,

            // if there is no last action time, set it to DateTimeOffset.Now and keep passesCooldown = true
            static (_, _) => DateTime.UtcNow,

            // if there is a value
            static (_, lastMessageTime, arg) =>
            {
                var now = DateTime.UtcNow;

                // passesCooldown = true if the last action is older than the minimum cooldown
                arg.PassesCooldown = (now - lastMessageTime) > arg.Cooldown;
                // update it if the last action time is older than the cooldown, otherwise keep the same cooldown 
                return arg.PassesCooldown ? now : lastMessageTime;
            },

            arg
        );

        return arg.PassesCooldown;
    }

    public int PruneEntries()
    {
        var removedCount = 0;
        var now = DateTime.UtcNow;

        foreach (var (snowflake, lastMessageTime) in _dict)
        {
            if (now - lastMessageTime > _cooldown)
            {
                if (_dict.TryRemove(KeyValuePair.Create(snowflake, lastMessageTime)))
                {
                    removedCount++;
                }
            }
        }

        return removedCount;
    }
}
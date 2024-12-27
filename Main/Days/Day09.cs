namespace AdventOfCode.Days;

internal class Day09 : DayBase<Day09, IReadOnlyList<long>>
{
    public override int Number => 9;
    
    protected override IReadOnlyList<long> ParseInput()
    {
        var inputSpan = Input.AsSpan();
        var digits = new List<long>();
        foreach (var digit in inputSpan)
        {
            if (digit is < '0' or > '9')
                continue;
            digits.Add(digit - '0');
        }
        return digits;
    }

    public override string FirstPart()
    {
        var digits = ParsedInput.Value;
        var allocationSpaceCount = digits.Sum();
        var allocation = new long?[allocationSpaceCount];
        var i = 0L;
        var currentFileId = 0L;
        var isFile = true;
        foreach (var digit in digits)
        {
            if (isFile)
            {
                for (var j = 0; j < digit; j++)
                {
                    allocation[i++] = currentFileId;
                }
                ++currentFileId;
                isFile = false;
            }
            else
            {
                i += digit;
                isFile = true;
            }
        }
        var left = 0;
        var right = allocation.Length - 1;
        while (left < right)
        {
            if (allocation[left] is not null) 
                left++;
            else if (allocation[right] is null)
                right--;
            else
            {
                allocation[left] = allocation[right];
                allocation[right] = null;
            }
        }
        return allocation
            .Select((id, index) => (id, index))
            .Sum(t => t.id is not null ? t.index * t.id : 0L)
            ?.ToString() ?? Consts.NoSolutionFound;
    }

    private abstract record AllocationNode(long Id, long Spaces)
    {
        internal AllocationNode? Previous { get; set; }
        internal AllocationNode? Next { get; set; }
        
        internal record File(long Id, long Spaces) : AllocationNode(Id, Spaces);
        internal record FreeSpace(long Id, long Spaces) : AllocationNode(Id, Spaces);
    }

    public override string SecondPart()
    {
        var digits = ParsedInput.Value;
        var currentFileId = 0L;
        var currentFreeSpaceId = 0L;
        var start = new AllocationNode.File(currentFileId++, digits[0]);
        AllocationNode current = start;
        var isFile = false;
        var nodeMap = new Dictionary<long, AllocationNode.File> { [0] = start };
        var spacesToFreeSpacesMap = new SortedDictionary<long, PriorityQueue<AllocationNode.FreeSpace, long>>();
        foreach (var digit in digits.Skip(1))
        {
            if (isFile)
            {
                var next = new AllocationNode.File(currentFileId++, digit) { Previous = current };
                nodeMap[next.Id] = next;
                current.Next = next;
                current = next;
                isFile = false;
            }
            else
            {
                var next = new AllocationNode.FreeSpace(currentFreeSpaceId++, digit) { Previous = current };
                AddItem(spacesToFreeSpacesMap, next.Spaces, next);
                current.Next = next;
                current = next;
                isFile = true;
            }
        }

        for (var i = currentFileId - 1; i >= 0; i--)
        {
            var currentFile = nodeMap[i];
            var maybeMinIdFreeSpaces = Enumerable.Range((int) currentFile.Spaces, 9 - (int) currentFile.Spaces + 1)
                .SelectMany(i => spacesToFreeSpacesMap.TryGetValue(i, out var nodes) ? [(AllocationNode.FreeSpace)nodes.Peek()] : Enumerable.Empty<AllocationNode.FreeSpace>())
                .MinBy(n => n.Id);

            if (maybeMinIdFreeSpaces is { } minIdFreeSpace && minIdFreeSpace.Id < currentFile.Id)
            {
                RemoveItem(spacesToFreeSpacesMap, minIdFreeSpace.Spaces, minIdFreeSpace);
                Rearrange(currentFile, minIdFreeSpace);
            }
        }
        
        var sum = 0L;
        var k = 0L;
        var currentForSum = (AllocationNode?) start;
        while (currentForSum is not null)
        {
            switch (currentForSum)
            {
                case AllocationNode.File { Id: var id, Spaces: var spaces }:
                    var upperBound = k + spaces;
                    for (var j = k; j < upperBound; j++)
                    {
                        sum += id * j;
                    }
                    k += spaces;
                    break;
                case AllocationNode.FreeSpace { Spaces: var spaces }:
                    k += spaces;
                    break;
            }
            currentForSum = currentForSum.Next;
        }
        
        return sum.ToString();

        void Rearrange(AllocationNode.File currentFile, AllocationNode currentNode)
        {
            var currentNodeIsPrevious = currentFile.Previous == currentNode;
            
            var previousToCurrentFile = (AllocationNode.FreeSpace?) currentFile.Previous;
            var nextToCurrentFile = (AllocationNode.FreeSpace?) currentFile.Next;
            if (previousToCurrentFile is not null && nextToCurrentFile is not null)
            {
                var newFreeSpace = new AllocationNode.FreeSpace(previousToCurrentFile.Id, previousToCurrentFile.Spaces + currentFile.Spaces + nextToCurrentFile.Spaces)
                {
                    Previous = previousToCurrentFile.Previous,
                    Next = nextToCurrentFile.Next
                };
                AddItem(spacesToFreeSpacesMap, newFreeSpace.Spaces, newFreeSpace);
                if (previousToCurrentFile.Previous is not null)
                {
                    previousToCurrentFile.Previous.Next = newFreeSpace;
                    RemoveItem(spacesToFreeSpacesMap, previousToCurrentFile.Spaces, previousToCurrentFile);
                }
                if (nextToCurrentFile.Next is not null)
                {
                    nextToCurrentFile.Next.Previous = newFreeSpace;
                    RemoveItem(spacesToFreeSpacesMap, nextToCurrentFile.Spaces, nextToCurrentFile);
                }
                
                if (currentNodeIsPrevious)
                    currentNode = newFreeSpace;
            }
            else if (previousToCurrentFile is not null)
            {
                var newFreeSpace = new AllocationNode.FreeSpace(previousToCurrentFile.Id, previousToCurrentFile.Spaces + currentFile.Spaces)
                {
                    Previous = previousToCurrentFile.Previous,
                    Next = null
                };
                AddItem(spacesToFreeSpacesMap, newFreeSpace.Spaces, newFreeSpace);
                if (previousToCurrentFile.Previous is not null)
                {
                    previousToCurrentFile.Previous.Next = newFreeSpace;
                    RemoveItem(spacesToFreeSpacesMap, previousToCurrentFile.Spaces, previousToCurrentFile);
                }
                
                if (currentNodeIsPrevious)
                    currentNode = newFreeSpace;
            } 
            else if (nextToCurrentFile is not null)
            {
                var newFreeSpace = new AllocationNode.FreeSpace(nextToCurrentFile.Id, currentFile.Spaces + nextToCurrentFile.Spaces)
                {
                    Previous = null,
                    Next = nextToCurrentFile.Next
                };
                AddItem(spacesToFreeSpacesMap, newFreeSpace.Spaces, newFreeSpace);
                if (nextToCurrentFile.Next is not null)
                {
                    nextToCurrentFile.Next.Previous = newFreeSpace;
                    RemoveItem(spacesToFreeSpacesMap, nextToCurrentFile.Spaces, nextToCurrentFile);
                }
            }
                
            var newPreviousFreeSpace = new AllocationNode.FreeSpace(-1, 0)
            {
                Previous = currentNode.Previous,
                Next = currentFile
            };
            var newNextFreeSpace = new AllocationNode.FreeSpace(currentNode.Id, currentNode.Spaces - currentFile.Spaces)
            {
                Previous = currentFile,
                Next = currentNode.Next
            };
            AddItem(spacesToFreeSpacesMap, newNextFreeSpace.Spaces, newNextFreeSpace);
            RemoveItem(spacesToFreeSpacesMap, currentNode.Spaces, (AllocationNode.FreeSpace) currentNode);
            currentFile.Previous = newPreviousFreeSpace;
            currentFile.Next = newNextFreeSpace;
            if (currentNode.Previous is not null)
                currentNode.Previous.Next = newPreviousFreeSpace;
            if (currentNode.Next is not null)
                currentNode.Next.Previous = newNextFreeSpace;
        }
    }
    
    private static void AddItem(SortedDictionary<long, PriorityQueue<AllocationNode.FreeSpace, long>> dictionary, long key, AllocationNode.FreeSpace value)
    {
        if (dictionary.TryGetValue(key, out var inner))
        {
            inner.Enqueue(value, value.Id);
        }
        else
        {
            var priorityQueue = new PriorityQueue<AllocationNode.FreeSpace, long>();
            priorityQueue.Enqueue(value, value.Id);
            dictionary[key] = priorityQueue;
        }
    }
    
    private static void RemoveItem(SortedDictionary<long, PriorityQueue<AllocationNode.FreeSpace, long>> dictionary, long key, AllocationNode.FreeSpace value)
    {
        if (!dictionary.TryGetValue(key, out var inner)) 
            return;
        inner.Remove(value, out _, out _);
        if (inner.Count == 0)
            dictionary.Remove(key);
    }
}
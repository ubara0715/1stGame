using UnityEngine;

public class Stage01_0 : StageBase
{
    [SerializeField] float limit = 30.0f;

    protected override void Start()
    {
        timeLimit = limit;

        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}

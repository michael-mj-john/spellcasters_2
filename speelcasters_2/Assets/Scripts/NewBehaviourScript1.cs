using UnityEngine;
using System.Collections;

interface ITeamOwned
{
    bool blue { get; set; }
    Transform owner { get; set; }
    void SetBlue(bool blue_);
    bool GetBlue();
}

interface IShield : ITeamOwned
{
    void DestroyShield();
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class VerifyStage : Regulus.Utility.IStage
{
    private string _Account;
    private string _Password;
    private Regulus.Remoting.INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> _Provider;

    public delegate void DoneCallback();
    public event DoneCallback SuccessEvent;
    public event DoneCallback FailEvent;

    public VerifyStage(string _Account, string _Password, Regulus.Remoting.INotifier<Regulus.Project.ItIsNotAGame1.Data.IVerify> providerNotice)
    {
        // TODO: Complete member initialization
        this._Account = _Account;
        this._Password = _Password;
        this._Provider = providerNotice;
    }
    void Regulus.Utility.IStage.Enter()
    {
        _Provider.Supply += _Provider_Supply;
    }

    void _Provider_Supply(Regulus.Project.ItIsNotAGame1.Data.IVerify obj)
    {
        obj.Login(_Account, _Password).OnValue += _Result;
    }

    private void _Result(bool obj)
    {
        if (obj)
            SuccessEvent();
        else
            FailEvent();
    }

    void Regulus.Utility.IStage.Leave()
    {
        _Provider.Supply -= _Provider_Supply;
    }

    void Regulus.Utility.IStage.Update()
    {

    }
}

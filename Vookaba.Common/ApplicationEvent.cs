﻿namespace Vookaba.Common
{
    public enum ApplicationEvent
    {
        //Boards events 10_xxx
        BoardCreate = 10_001,
        BoardDelete = 10_002,
        BoardEdit = 10_100,

        //Account events 20_xxx
        AccountCreate = 20_001,
        AccountLogin = 20_002,
        AccountRoleRemove = 20_101,
        AccountRoleAdd = 20_102,
        AccountBoardsPermissionAdd = 20_111,
        AccountBoardsPermissionRemove = 20_112,
        InvitationCreate = 20_201,

        //thread events 30_xxx
        ThreadIsPinnedChanged = 30_001,
        ThreadIsLockedChanged = 30_005,

        //posts events 40_xxx
        PostDelete = 40_001,
        PostBulkDelete = 40_002,

        //bans events 45_xxx
        BanCreated = 45_001,
        BanRemoved = 45_002,

        //misc. 60_xxx
        ModLogCreated = 60_001,
        ModLogCreatingFailed = 60_002,
        ApplicationProlem = 60_100
    }
}

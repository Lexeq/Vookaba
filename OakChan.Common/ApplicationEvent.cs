namespace OakChan.Common
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
        InvitationCreate = 20_201

    }
}

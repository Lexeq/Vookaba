namespace OakChan.Common
{
    public enum ApplicationEvent
    {
        //Boards events
        BoardCreate = 10_001,
        BoardDelete = 10_002,
        BoardEdit = 10_100,

        //Account events
        AccountCreate = 20_001,
        AccountLogin = 20_002,
        AccountChangeRole = 20_101,
        AccountChangeBoardsPermission = 20_102,

        //Invitation event
        InvitationCreate = 30_001
    }
}

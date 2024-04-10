namespace Gregghz.DisplayManager.Windows.Native;

public static class Constants
{
  public const int ENUM_CURRENT_SETTINGS = -1;
  public const uint EDD_GET_DEVICE_INTERFACE_NAME = 0x00000001;

  public const int CDS_NONE = 0x00;
  public const int CDS_UPDATEREGISTRY = 0x01;
  public const int CDS_TEST = 0x02;
  public const int CDS_FULLSCREEN = 0x04;
  public const int CDS_GLOBAL = 0x08;
  public const int CDS_SET_PRIMARY = 0x10;
  public const int CDS_VIDEOPARAMETERS = 0x20;
  public const int CDS_ENABLE_UNSAFE_MODES = 0x100;
  public const int CDS_DISABLE_UNSAFE_MODES = 0x200;
  public const int CDS_RESET = 0x40000000;
  public const int CDS_RESET_EX = 0x20000000;
  public const int CDS_NORESET = 0x10000000;

  public const int DISP_CHANGE_SUCCESSFUL = 0;
  public const int DISP_CHANGE_RESTART = 1;
  public const int DISP_CHANGE_FAILED = -1;
  public const int DISP_CHANGE_BADMODE = -2;
  public const int DISP_CHANGE_NOTUPDATED = -3;
  public const int DISP_CHANGE_BADFLAGS = -4;
  public const int DISP_CHANGE_BADPARAM = -5;
  public const int DISP_CHANGE_BADDUALVIEW = -6;
}
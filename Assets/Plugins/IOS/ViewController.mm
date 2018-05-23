//
//  ViewController.m
//  Test
//
//  Created by Wang WenFei on 1/8/16.
//  Copyright © 2016 cloud. All rights reserved.
//

#import "ViewController.h"

#import "UnityAppController.h"
#import "WXApi.h"
#import "AMapFoundationKit/AMapFoundationKit.h"
#import "AMapLocationKit/AMapLocationKit.h"
#import "MAMapKit/MAGeometry.h"


// 授权域：通俗讲就是接口的使用权限
static NSString *kAuthScope  = @"snsapi_message,snsapi_userinfo,snsapi_friend,snsapi_contact";
static NSString *kAuthState  = @"123321";
//const static NSString *APIKey = @"5b4e434867ab73811565709627da1ccf";  //高德地图的key

extern "C"{
    //获取俩个经纬度的距离
    double _CalcDistanceBetweenCoor(double la_1,double lo_1,double la_2,double lo_2)
    {
        CLLocationCoordinate2D coor1;
        coor1.latitude=la_1;
        coor1.longitude=lo_1;
        CLLocationCoordinate2D coor2;
        coor2.latitude=la_2;
        coor2.longitude=lo_2;
        MAMapPoint mapPointA = MAMapPointForCoordinate(coor1);
        MAMapPoint mapPointB = MAMapPointForCoordinate(coor2);
        return MAMetersBetweenMapPoints(mapPointA, mapPointB);
    }
    
    // 登录授权
    void _WXOpenAuthReq()
    {
        //构造SendAuthReq结构体
        SendAuthReq* req    =[[SendAuthReq alloc]init];
        req.scope           = kAuthScope;
        req.state           = kAuthState;
        // req.openID          = kAuthOpenID;
        //第三方向微信终端发送一个SendAuthReq消息结构
        [WXApi sendReq:req];
    }
    
    //简单文本分享
    void _WXOpenShareReq(const char* url ,const char* title,const char* discription ,int type)
    {
        //NSLog(@";;;;开始分享''''''");
        WXMediaMessage *message = [WXMediaMessage message];
        message.title           = [NSString stringWithUTF8String:title];
        message.description     = [NSString stringWithUTF8String:discription];
        
        [message setThumbImage:[UIImage imageNamed:@"icon.png"]];
        // NSLog(@"imagr;;;;开始分享''''''");
        WXWebpageObject *webpageObject=[WXWebpageObject object ];
        webpageObject.webpageUrl=[NSString stringWithUTF8String:url];
        message.mediaObject=webpageObject;
        
        SendMessageToWXReq *req=[[SendMessageToWXReq alloc ]init ];
        req.bText=NO;
        req.message=message;
        if (type==0) {
            req.scene = WXSceneSession;
        }if (type==1) {
            req.scene = WXSceneTimeline;
        }if (type==2) {
            req.scene = WXSceneFavorite;
        }
        // 目标场景
        // 发送到聊天界面  WXSceneSession
        // 发送到朋友圈    WXSceneTimeline
        // 发送到微信收藏  WXSceneFavorite
        
        [WXApi sendReq:req];
        // NSLog(@";------开始分享'-----");
    }
    
    void _CopyTextToClipboard(const char *textList)
    {
        NSString *text = [NSString stringWithUTF8String: textList] ;
        UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
        pasteboard.string = text;
        
    }
    UIImage* thumbImageWithImage(UIImage *scImg,CGSize limitSize )
    {
        if (scImg.size.width <= limitSize.width && scImg.size.height <= limitSize.height) {
            return scImg;
        }
        CGSize thumbSize;
        if (scImg.size.width / scImg.size.height > limitSize.width / limitSize.height) {
            thumbSize.width = limitSize.width;
            thumbSize.height = limitSize.width / scImg.size.width * scImg.size.height;
        }
        else {
            thumbSize.height = limitSize.height;
            thumbSize.width = limitSize.height / scImg.size.height * scImg.size.width;
        }
        UIGraphicsBeginImageContext(thumbSize);
        [scImg drawInRect:(CGRect){CGPointZero,thumbSize}];
        UIImage *thumbImg = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
        return thumbImg;
    }
    
    
}

extern "C" void UnitySendMessage(const char *, const char *, const char *);

ViewController* g_ViewController = nil;

@interface ViewController ()

@end

@implementation ViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view, typically from a nib.
    
    //    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    //    [self buy:ITEM_0];
}

-(void)Init
{
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)buy:(int)type
{
    buyType = type;
    
    if ([SKPaymentQueue canMakePayments]) {
        
        NSLog(@"允许程序内付费购买");
        [self RequestProductData];
    }
    else
    {
        NSLog(@"不允许程序内付费购买");
        UIAlertView *alerView =  [[UIAlertView alloc] initWithTitle:@"提示"
                                                            message:@"您的手机没有打开程序内付费购买"
                                                           delegate:nil cancelButtonTitle:NSLocalizedString(@"关闭",nil) otherButtonTitles:nil];
        
        [alerView show];
    }
}

-(void)RequestProductData
{
    NSLog(@"---------请求对应的产品信息------------");
    // NSArray *product = nil;
    NSString * product;
    NSLog(@"产品ID：%d",buyType );
    product = [NSString stringWithFormat:@"sxmj_anhui_%d",buyType];
    NSLog(@"产品在ios的商品ID：%@",product );
    NSSet *nsset = [NSSet setWithArray:@[product]];
    SKProductsRequest *request=[[SKProductsRequest alloc] initWithProductIdentifiers: nsset];
    request.delegate=self;
    [request start];
}

//<SKProductsRequestDelegate> 请求协议
//收到的产品信息
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response{
    
    NSLog(@"-----------收到产品反馈信息--------------");
    NSArray *myProduct = response.products;
    NSLog(@"产品Product ID:%@",response.invalidProductIdentifiers);
    NSLog(@"产品付费数量: %d", (int)[myProduct count]);
    // populate UI
    for(SKProduct *product in myProduct)
    {
        NSLog(@"product info");
        NSLog(@"SKProduct 描述信息%@", [product description]);
        NSLog(@"产品标题 %@" , product.localizedTitle);
        NSLog(@"产品描述信息: %@" , product.localizedDescription);
        NSLog(@"价格: %@" , product.price);
        NSLog(@"Product id: %@" , product.productIdentifier);
    }
    SKPayment *payment = nil;
    payment  = [SKPayment paymentWithProduct:myProduct[0]];
    NSLog(@"---------发送购买请求------------");
    [[SKPaymentQueue defaultQueue] addPayment:payment];
}



//弹出错误信息
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
    NSLog(@"-------弹出错误信息----------");
    UIAlertView *alerView =  [[UIAlertView alloc] initWithTitle:NSLocalizedString(@"Alert",NULL) message:[error localizedDescription]
                                                       delegate:nil cancelButtonTitle:NSLocalizedString(@"Close",nil) otherButtonTitles:nil];
    [alerView show];
}

-(void) requestDidFinish:(SKRequest *)request
{
    NSLog(@"----------反馈信息结束--------------");
    
}

-(void) PurchasedTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----PurchasedTransaction----");
    NSArray *transactions =[[NSArray alloc] initWithObjects:transaction, nil];
    [self paymentQueue:[SKPaymentQueue defaultQueue] updatedTransactions:transactions];
}

//<SKPaymentTransactionObserver> 千万不要忘记绑定，代码如下：
//----监听购买结果
//[[SKPaymentQueue defaultQueue] addTransactionObserver:self];

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions//交易结果
{
    NSLog(@"-----paymentQueue--------");
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
            {//交易完成
                [self completeTransaction:transaction];
                NSLog(@"-----交易完成 --------");
                
            } break;
            case SKPaymentTransactionStateFailed://交易失败
            {
                [self failedTransaction:transaction];
                NSLog(@"-----交易失败 --------");
                UIAlertView *alerView2 =  [[UIAlertView alloc] initWithTitle:@"提示"
                                                                     message:@"购买失败，请重新尝试购买"
                                                                    delegate:nil cancelButtonTitle:NSLocalizedString(@"关闭",nil) otherButtonTitles:nil];
                
                [alerView2 show];
                
            }break;
            case SKPaymentTransactionStateRestored://已经购买过该商品
                [self restoreTransaction:transaction];
                NSLog(@"-----已经购买过该商品 --------");
                break;
            case SKPaymentTransactionStatePurchasing:      //商品添加进列表
                NSLog(@"-----商品添加进列表 --------");
                break;
            default:
                NSString *ssp=@"";
                 UnitySendMessage("SDKMgr", "OnPurchaseFinish",[ssp UTF8String] );
                
                break;
        }
    }
}

/**
 *  验证购买，避免越狱软件模拟苹果请求达到非法购买问题
 *
 */
-(void)verifyPurchaseWithPaymentTransaction
{
    //从沙盒中获取交易凭证并且拼接成请求体数据
    NSURL *receiptUrl=[[NSBundle mainBundle] appStoreReceiptURL];
    NSData *receiptData=[NSData dataWithContentsOfURL:receiptUrl];
    NSLog(@"--------------------------------编码前receiptString %@",receiptData);
    NSLog(@"--------------------------------编码前receiptString %lu",(unsigned long)receiptData.length);
    NSString *receiptString=[receiptData base64EncodedStringWithOptions:NSDataBase64EncodingEndLineWithLineFeed];//转化为base64字符串
    // NSLog(@"--------------------------------编码后receiptString %@",receiptString);
    NSLog(@"--------------------------------编码后的长度： %d",(int)receiptString.length);
    NSLog(@"--------------------------------准备回调unity方法");
    if(receiptString.length>0)
    {
        NSLog(@"--------------------------------回调unity方法");
        NSLog(@"--------------------------------编码前receiptString %@",receiptString);
        UnitySendMessage("SDKMgr", "OnPurchaseFinish", [receiptString UTF8String]);
    }
}

- (void) completeTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@"-----completeTransaction--------");
    
    // Your application should implement these two methods.
    NSString *product = transaction.payment.productIdentifier;
    
    
    
    if ([product length] > 0)
    {
        NSArray *tt = [product componentsSeparatedByString:@"_"];
        NSString *chargeid = [tt lastObject];
        
        if ([chargeid length] > 0)
        {
            
            [self verifyPurchaseWithPaymentTransaction];
        }
    }
    
    // Remove the transaction from the payment queue.
    
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

//记录交易
//-(void)recordTransaction:(NSString *)product{
//   NSLog(@"-----记录交易--------");
//}

//处理下载内容
//-(void)provideContent:(NSString *)product{
//    NSLog(@"-----下载--------");
//}

- (void) failedTransaction: (SKPaymentTransaction *)transaction{
    NSLog(@"failedTransaction 失败");
    if (transaction.error.code != SKErrorPaymentCancelled)
    {
        
    }
    [[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}

-(void) paymentQueueRestoreCompletedTransactionsFinished: (SKPaymentTransaction *)transaction{
    
}

- (void) restoreTransaction: (SKPaymentTransaction *)transaction
{
    NSLog(@" 交易恢复处理");
    
}

-(void) paymentQueue:(SKPaymentQueue *) paymentQueue restoreCompletedTransactionsFailedWithError:(NSError *)error{
    NSLog(@"-------paymentQueue----");
}

#pragma mark connection delegate
- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data
{
    NSLog(@"didReceiveData %@",  [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding]);
}
- (void)connectionDidFinishLoading:(NSURLConnection *)connection{
    
}

- (void)connection:(NSURLConnection *)connection didReceiveResponse:(NSURLResponse *)response{
    
    NSLog(@"didReceiveResponse %d",  [(NSHTTPURLResponse *)response statusCode]);
    
    switch([(NSHTTPURLResponse *)response statusCode])
    {
        case 200:
        case 206:
            break;
        case 304:
            break;
        case 400:
            break;
        case 404:
            break;
        case 416:
            break;
        case 403:
            break;
        case 401:
        case 500:
            break;
        default:
            break;
    }
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error {
    NSLog(@"didFailWithError test");
}

- (void)objc_copyTextToClipboard : (NSString*)text
{
    UIPasteboard *pasteboard = [UIPasteboard generalPasteboard];
    pasteboard.string = text;
}

@end

extern "C"
{
    void _UniIAPCharge(int type);
    void _copyTextToClipboard(char* textlist);
}

void _UniIAPCharge(int type)
{
    if(g_ViewController == nil)
    {
        g_ViewController = [ViewController alloc];
        [g_ViewController Init];
    }
    
    if(g_ViewController != nil)
    {
        [g_ViewController buy: type];
    }
    else
    {
        NSLog(@"_UniIAPCharge failed!");
    }
}

void _copyTextToClipboard(char* textlist)
{
    if(g_ViewController == nil)
    {
        g_ViewController = [ViewController alloc];
        [g_ViewController Init];
    }
    
    if(g_ViewController != nil)
    {
        NSString *text = [NSString stringWithUTF8String: textlist] ;
        [g_ViewController objc_copyTextToClipboard: text];
    }
}

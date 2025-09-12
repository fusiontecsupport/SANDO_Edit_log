CREATE PROCEDURE [dbo].[z_pr_CHPOE07_XML_Detail_Insert_Assgn] @Pxmlid int,
@PDHID INT OUTPUT

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @xml XML
	Select @xml = XMLData From XMLwithOpenXML Where XMLId = @Pxmlid

	INSERT INTO XML_CHPOE07Payload_DocumentHeader(SenderIdQualifier,SenderId,ReceiverIdQualifier,ReceiverId,
	VersionNumber,TestProductionFlag,MessageId,ControlNumber,SentDate,SentTime,PCSCommonRefNumber,XMLId)
    SELECT
    --Customer.value('@SenderIdQualifier','VARCHAR(10)') AS Id, --ATTRIBUTE
    DocumentHeader.value('(SenderIdQualifier/text())[1]','VARCHAR(10)') AS SenderIdQualifier, --TAG
    DocumentHeader.value('(SenderId/text())[1]','VARCHAR(10)') AS SenderId, --TAG
	DocumentHeader.value('(ReceiverIdQualifier/text())[1]','VARCHAR(10)') AS ReceiverIdQualifier, --TAG
	DocumentHeader.value('(ReceiverId/text())[1]','VARCHAR(15)') AS ReceiverId, --TAG
	DocumentHeader.value('(VersionNumber/text())[1]','VARCHAR(15)') AS VersionNumber, --TAG
	DocumentHeader.value('(TestProductionFlag/text())[1]','VARCHAR(10)') AS TestProductionFlag, --TAG
	DocumentHeader.value('(MessageId/text())[1]','VARCHAR(15)') AS MessageId, --TAG
	DocumentHeader.value('(ControlNumber/text())[1]','VARCHAR(10)') AS ControlNumber, --TAG
	DocumentHeader.value('(SentDate/text())[1]','VARCHAR(10)') AS SentDate, --TAG
	DocumentHeader.value('(SentTime/text())[1]','VARCHAR(10)') AS SentTime, --TAG
	DocumentHeader.value('(PCSCommonRefNumber/text())[1]','VARCHAR(10)') AS PCSCommonRefNumber, --TAG
	@Pxmlid
    FROM
    @xml.nodes('/CHPOE07Payload/DocumentHeader')AS TEMPTABLE(DocumentHeader)

	SET @PDHID = SCOPE_IDENTITY()

	IF (@PDHID > 0)
		INSERT INTO XML_CHPOE07Payload_Leo(DHId,MessageType,SiteID,SBNo,SBDate,RotationNo,RotationDate,LEODate,
		NatureOfCargo,GatewayPort,TransCd,CinNo)
		SELECT @PDHID,
		--Customer.value('@SenderIdQualifier','VARCHAR(10)') AS Id, --ATTRIBUTE
		Leo.value('(MessageType/text())[1]','VARCHAR(10)') AS MessageType, --TAG
		Leo.value('(SiteID/text())[1]','VARCHAR(10)') AS SiteID, --TAG
		Leo.value('(SBNo/text())[1]','VARCHAR(25)') AS SBNo, --TAG
		Leo.value('(SBDate/text())[1]','VARCHAR(10)') AS SBDate, --TAG
		Leo.value('(RotationNo/text())[1]','VARCHAR(15)') AS RotationNo, --TAG
		Leo.value('(RotationDate/text())[1]','VARCHAR(10)') AS RotationDate, --TAG
		Leo.value('(LEODate/text())[1]','VARCHAR(10)') AS LEODate, --TAG
		Leo.value('(NatureOfCargo/text())[1]','VARCHAR(10)') AS NatureOfCargo, --TAG
		Leo.value('(GatewayPort/text())[1]','VARCHAR(15)') AS GatewayPort, --TAG
		Leo.value('(TransCd/text())[1]','VARCHAR(10)') AS TransCd, --TAG
		Leo.value('(CinNo/text())[1]','VARCHAR(25)') AS CinNo --TAG
		FROM
		@xml.nodes('/CHPOE07Payload/Leo')AS TEMPTABLE(Leo)




	Update XMLwithOpenXML Set XMLStatus = 1 Where XMLId = @Pxmlid

END

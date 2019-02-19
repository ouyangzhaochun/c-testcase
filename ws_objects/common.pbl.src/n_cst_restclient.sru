$PBExportHeader$n_cst_restclient.sru
forward
global type n_cst_restclient from nonvisualobject
end type
end forward

global type n_cst_restclient from nonvisualobject
end type
global n_cst_restclient n_cst_restclient

type variables
RESTClient inv_restclient
end variables

forward prototypes
public function integer of_sendpostrequest (string as_url, string as_data, ref jsonpackage anv_pak)
public function integer of_sendgetrequest (string as_url, ref jsonpackage anv_pak)
public function integer of_sendputrequest (string as_url, string as_data, ref jsonpackage anv_pak)
public function integer of_senddeleterequest (string as_url, string as_data, ref jsonpackage anv_pak)
public function integer of_sendpatchrequest (string as_url, string as_data, ref jsonpackage anv_pak)
public function integer of_execute_func (string as_url, str_arm astr_am, ref jsonpackage anv_pak)
public function integer of_createjson_request (str_arm astr_am, ref string as_json)
end prototypes

public function integer of_sendpostrequest (string as_url, string as_data, ref jsonpackage anv_pak);Integer li_return
String   ls_json
String   ls_error

inv_restclient.SetRequestHeader("Content-Type", "application/json;charset=UTF-8")

li_return = inv_restclient.SendPostRequest(as_url, as_data, ls_json)
li_return = inv_restclient.GetResponseStatusCode() 

IF  li_return = 500 Then
		Messagebox("Error", "The server is not responding.")
		Return -1
	Else
		ls_error = anv_pak.LoadString(ls_json)
		IF ls_error = '' Then
		Else
			Messagebox("Error", "Failed to load the JSON file.")
			Return -1
		End IF
End IF
			
return 1
end function

public function integer of_sendgetrequest (string as_url, ref jsonpackage anv_pak);Integer li_return
String   ls_json
String   ls_error

inv_restclient.SetRequestHeader("Content-Type", "application/json;charset=UTF-8")

inv_restclient.SendGetRequest(as_url, ls_json)
li_return = inv_restclient.GetResponseStatusCode() 

IF  li_return = 500 Then
		Messagebox("Error", "The server is not responding.")
		Return -1
	Else
		ls_error = anv_pak.LoadString(ls_json)
		IF ls_error = '' Then
		Else
			Messagebox("Error", "Failed to load the JSON file.")
			Return -1
		End IF
End IF
			
return 1
end function

public function integer of_sendputrequest (string as_url, string as_data, ref jsonpackage anv_pak);Integer li_return
String   ls_json
String   ls_error

inv_restclient.SetRequestHeader("Content-Type", "application/json;charset=UTF-8")

inv_restclient.SendPutRequest(as_url, as_data, ls_json)
li_return = inv_restclient.GetResponseStatusCode() 

IF  li_return = 500 Then
		Messagebox("Error", "The server is not responding.")
		Return -1
	Else
		ls_error = anv_pak.LoadString(ls_json)
		IF ls_error = '' Then
		Else
			Messagebox("Error", "Failed to load the JSON file.")
			Return -1
		End IF
End IF
			
return 1
end function

public function integer of_senddeleterequest (string as_url, string as_data, ref jsonpackage anv_pak);Integer li_return
String   ls_json
String   ls_error

inv_restclient.SetRequestHeader("Content-Type", "application/json;charset=UTF-8")

inv_restclient.SendDeleteRequest(as_url, as_data, ls_json)
li_return = inv_restclient.GetResponseStatusCode() 

IF  li_return = 500 Then
		Messagebox("Error", "The server is not responding.")
		Return -1
Else
	ls_error = anv_pak.LoadString(ls_json)
	IF ls_error = '' Then
	Else
		Messagebox("Error", "Failed to load the JSON file.")
		Return -1
	End IF
End IF
			
return 1
end function

public function integer of_sendpatchrequest (string as_url, string as_data, ref jsonpackage anv_pak);Integer li_return
String   ls_json
String   ls_error

inv_restclient.SetRequestHeader("Content-Type", "application/json;charset=UTF-8")

inv_restclient.SendPatchRequest(as_url, as_data, ls_json)
li_return = inv_restclient.GetResponseStatusCode() 

IF  li_return = 500 Then
		Messagebox("Error", "The server is not responding.")
		Return -1
	Else
		ls_error = anv_pak.LoadString(ls_json)
		IF ls_error = '' Then
		Else
			Messagebox("Error", "Failed to load the JSON file.")
			Return -1
		End IF
End IF
			
return 1
end function

public function integer of_execute_func (string as_url, str_arm astr_am, ref jsonpackage anv_pak);String ls_json
String ls_request_type
Integer li_ret

IF Not isvalid(anv_pak) Then anv_pak  = Create jsonpackage

ls_request_type = astr_am.request_type
of_createjson_request(astr_am, ls_json)

IF isnull(ls_request_type) Or ls_request_type = "" Then
	ls_request_type = "post"
End IF

Choose Case Lower(ls_request_type)
	Case "get"
		li_ret = of_sendgetrequest(as_url, anv_pak)
	Case "post"
		li_ret = of_sendpostrequest(as_url, ls_json, anv_pak)
	Case "put"
		li_ret = of_sendputrequest(as_url, ls_json, anv_pak)
	Case "delete"
		li_ret = of_senddeleterequest(as_url, ls_json, anv_pak)
	Case "patch"
		li_ret = of_sendpatchrequest(as_url, ls_json, anv_pak)
End Choose

Return li_ret
end function

public function integer of_createjson_request (str_arm astr_am, ref string as_json);String ls_sql[]
String ls_arm[]
String ls_dwname[]
String ls_type[]
String ls_request_type
JsonPackage lnv_jpack
Int li_cnt
DataWindow ldw_am[]
DataWindowChild ldwc_am[]

lnv_jpack = Create jsonpackage

ls_request_type = astr_am.request_type
IF Lower(ls_request_type) = "get" Then
	as_json = ""
	Return 1
End IF

ls_sql = astr_am.sql_arm
ls_arm = astr_am.s_arm
ldw_am = astr_am.dw_arm
ldwc_am = astr_am.dwc_arm
ls_dwname = astr_am.s_dwname
ls_type = astr_am.s_type

For li_cnt = 1 To UpperBound(ls_sql)
	lnv_jpack.SetValue("sql"+String(li_cnt), ls_sql[li_cnt], False)
Next

For li_cnt = 1 To UpperBound(ls_arm)
	lnv_jpack.SetValue("arm"+String(li_cnt), ls_arm[li_cnt], False)
Next

For li_cnt = 1 To UpperBound(ls_type)
	lnv_jpack.SetValue("type"+String(li_cnt), ls_type[li_cnt], False)
Next

For li_cnt = 1 To UpperBound(ldw_am)
	lnv_jpack.SetValue("dw"+String(li_cnt), ldw_am[li_cnt], True)
Next

For li_cnt = 1 To UpperBound(ls_dwname)
	lnv_jpack.SetValue("dwname"+String(li_cnt), ls_dwname[li_cnt], False)	
Next

For li_cnt = 1 To UpperBound(ldwc_am)
	lnv_jpack.SetValue("dwc"+String(li_cnt), ldwc_am[li_cnt], True)
Next

as_json = lnv_jpack.GetJsonString()

return 1
end function

on n_cst_restclient.create
call super::create
TriggerEvent( this, "constructor" )
end on

on n_cst_restclient.destroy
TriggerEvent( this, "destructor" )
call super::destroy
end on

event constructor;inv_restclient = Create RESTClient 
end event


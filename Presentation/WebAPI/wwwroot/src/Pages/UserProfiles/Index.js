/*
 ----------------------------------------------------------------------------
 Developer: Ismail Hamzah
 Email: go2ismail@gmail.com
 ----------------------------------------------------------------------------
*/

import { computed, createApp, reactive, ref, onMounted } from 'vue'
import { useAccessManager } from 'useAccessManager'
import { useStorageManager } from 'useStorageManager'
import { useAxios } from 'useAxios'
import { usePagedList } from 'usePagedList'
import indotable from 'indotable'
import indoloading from 'indoloading'
import indobutton from 'indobutton'

const app = createApp({
    components: {
        indotable,
        indoloading,
        indobutton
    },
    setup() {
        const { checkPageAccess, hasClaimAccess } = useAccessManager()
        const { clearStorage, getUserId } = useStorageManager()
        const userId = ref(getUserId())
        const { isSubmitting, request } = useAxios()
        const pageLoading = ref(true)
        const successMessage = ref('')
        const columns = ref([
            { columnKey: 'firstName', columnName: 'FirstName', columnCaption: 'FirstName', columnSort: null },
            { columnKey: 'lastName', columnName: 'LastName', columnCaption: 'LastName', columnSort: null },
            { columnKey: 'email', columnName: 'Email', columnCaption: 'Email', columnSort: null },
            { columnKey: 'emailConfirmed', columnName: 'EmailConfirmed', columnCaption: 'EmailConfirmed', columnSort: null },
            { columnKey: 'isBlocked', columnName: 'IsBlocked', columnCaption: 'IsBlocked', columnSort: null },
            { columnKey: 'isDeleted', columnName: 'IsDeleted', columnCaption: 'IsDeleted', columnSort: null }
        ])
        const {
            paged,
            apiUrl
        } = usePagedList('/UserProfile/GetUsersByUserId', `userId=${userId.value}`)
        const imageUrl = ref(null)
        const selectedProfilePicture = ref(null)
        const pageMode = reactive({
            create: false,
            read: true,
            update: false,
            delete: false
        })
        const formData = reactive({ ...paged.selectedRow })
        const errorData = reactive({})
        const changePageMode = (mode) => {
            successMessage.value = ''
            const modes = ['create', 'read', 'update', 'delete']
            modes.forEach(m => pageMode[m] = m === mode)
        }
        const claimLookup = ref([])
        const roleLookup = ref([])
        const userProfileClaims = ref({})
        const userProfileRoles = ref({})
        const claimsTypesSelected = ref({})
        const rolesTypesSelected = ref({})

        const handleClickClose = () => {
            if (pageMode.create) {
                paged.selectedRow = null
            } else {
                Object.assign(formData, paged.selectedRow)
            }
            changePageMode('read')
            successMessage.value = ''
        }

        const refreshClaimTable = () => {
            userProfileClaims.value = claimLookup.value.reduce((entities, claim) => {
                const [entity, access] = claim.text.split(':')
                if (!entities[entity]) {
                    entities[entity] = []
                }
                return entities
            }, {})

            claimsTypesSelected.value = Object.keys(userProfileClaims.value).reduce((acc, entity) => {
                acc[entity] = {
                    Create: false,
                    Read: false,
                    Update: false,
                    Delete: false
                }
                return acc
            }, {})

            if (paged.selectedRow.id) {
                let claims = paged.data.find(item => item.id === paged.selectedRow.id)?.claims
                if (claims) {
                    let groupedClaims = claims.reduce((acc, claim) => {
                        const [entity, access] = claim.split(':')
                        if (!acc[entity]) {
                            acc[entity] = []
                        }
                        acc[entity].push(access)
                        return acc
                    }, {})

                    userProfileClaims.value = claimLookup.value.reduce((entities, claim) => {
                        const [entity, access] = claim.text.split(':')
                        if (!entities[entity]) {
                            entities[entity] = []
                        }
                        return entities
                    }, {})

                    for (const entity in groupedClaims) {
                        if (userProfileClaims.value[entity]) {
                            userProfileClaims.value[entity] = groupedClaims[entity]
                        }
                    }

                    claimsTypesSelected.value = Object.keys(userProfileClaims.value).reduce((acc, entity) => {
                        acc[entity] = {
                            Create: userProfileClaims.value[entity]?.includes('Create') || false,
                            Read: userProfileClaims.value[entity]?.includes('Read') || false,
                            Update: userProfileClaims.value[entity]?.includes('Update') || false,
                            Delete: userProfileClaims.value[entity]?.includes('Delete') || false
                        }
                        return acc
                    }, {})

                }
            }
        }

        const refreshRoleTable = () => {
            userProfileRoles.value = roleLookup.value.map(role => role.text)
            rolesTypesSelected.value = userProfileRoles.value.reduce((acc, role) => {
                acc[role] = false
                return acc
            }, {})

            if (paged.selectedRow.roles) {
                rolesTypesSelected.value = userProfileRoles.value.reduce((acc, role) => {
                    acc[role] = paged.selectedRow.roles.includes(role)
                    return acc
                }, {})
            }
        }


        const handleClickEdit = () => {
            changePageMode('update')
            Object.assign(formData, paged.selectedRow)
        }


        const handleClickDelete = async () => {

            changePageMode('delete')
            Swal.fire({
                title: 'Are you sure?',
                text: "You won't be able to revert this!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!',
                cancelButtonText: 'Cancel',
                allowOutsideClick: false
            }).then(async (result) => {

                if (result.isConfirmed) {

                    try {
                        changePageMode('read')

                        const response = await request('delete', `/UserProfile/DeleteUserProfile`, {
                            email: paged.selectedRow.email,
                            userId: userId.value
                        })

                        if (response?.data?.code === 200) {

                            const response = await request('post', '/Account/Logout', { userId: userId.value })
                            clearStorage()
                            Swal.fire({
                                title: 'Deleted!',
                                text: 'Your data has been deleted.',
                                icon: 'success',
                                confirmButtonText: 'Ok',
                                allowOutsideClick: false
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    window.location.href = '/Index'
                                }
                            })
                        } else {
                            Swal.fire({
                                title: 'Error!',
                                text: `${response?.data?.content?.message}`,
                                icon: 'error',
                                confirmButtonText: 'Ok',
                                allowOutsideClick: false
                            })
                        }

                    } catch (error) {

                        Swal.fire({
                            title: 'Error!',
                            text: `Delete: ${error?.response?.data?.message || error}`,
                            icon: 'error',
                            confirmButtonText: 'Ok',
                            allowOutsideClick: false
                        })
                    }
                }

                if (!result.isConfirmed) {
                    changePageMode('read')
                }
            })
        }


        const formValidation = () => {
            let valid = true

            errorData.email = ''
            errorData.firstName = ''
            errorData.lastName = ''

            if (!formData.email) {
                errorData.email = 'Email is required.'
                valid = false
            }
            if (!formData.firstName) {
                errorData.firstName = 'FirstName is required.'
                valid = false
            }
            if (!formData.lastName) {
                errorData.lastName = 'LastName is required.'
                valid = false
            }

            return valid
        }


        const handleSubmitForm = async () => {
            if (!formValidation()) return

            try {

                let selectedRoles = []

                for (const role in rolesTypesSelected.value) {
                    if (rolesTypesSelected.value[role]) {
                        selectedRoles.push(role)
                    }
                }


                const response = await request(
                    'post',
                    pageMode.create ? '/UserProfile/CreateUserProfile' : '/UserProfile/UpdateUserProfile',
                    {
                        ...(pageMode.create ? {} : { id: formData.id }),
                        userId: userId.value,
                        email: formData.email,
                        password: formData.password,
                        confirmPassword: formData.confirmPassword,
                        firstName: formData.firstName,
                        lastName: formData.lastName,
                        emailConfirmed: formData.emailConfirmed ?? false,
                        isBlocked: formData.isBlocked ?? false,
                        isDeleted: false,
                        roles: selectedRoles
                    }
                )

                successMessage.value = `${pageMode.create ? 'Create successfull!' : 'Update successfull!'}`

                Swal.fire({
                    title: 'Success!',
                    text: `${successMessage.value}`,
                    icon: 'success',
                    confirmButtonText: 'Ok',
                    allowOutsideClick: false
                }).then(async (result) => {
                    if (result.isConfirmed) {
                        if (pageMode.create) {
                            changePageMode('update')
                            await getDataTable()
                            paged.selectedRow = {}
                            formData.id = response?.data?.content?.id
                            formData.password = null
                            formData.confirmPassword = null
                            Object.assign(paged.selectedRow, formData)
                        } else {
                            await getDataTable()
                            formData.password = null
                            formData.confirmPassword = null
                            Object.assign(paged.selectedRow, formData)
                        }

                        refreshClaimTable()

                    }
                })
            } catch (error) {
                Swal.fire({
                    title: 'Error!',
                    text: `${pageMode.create ? 'Create' : 'Update'}: ${error?.response?.data?.message || error}`,
                    icon: 'error',
                    confirmButtonText: 'Ok',
                    allowOutsideClick: false
                })
            } finally {
                isSubmitting.value = false
            }
        }

        const handleProfilePictureChange = async (event) => {
            const file = event.target.files[0]
            if (file) {
                selectedProfilePicture.value = file
                imageUrl.value = URL.createObjectURL(selectedProfilePicture.value)
            }
        }

        const uploadProfilePicture = async () => {
            try {
                const email = paged.data?.[0]?.email
                const formData = new FormData()
                formData.append('imageFile', selectedProfilePicture.value)
                const response = await request('post', `/UserProfile/UploadProfilePictureMember?userEmail=${email}`, formData)
                Swal.fire({
                    title: 'Success!',
                    text: 'Profile picture uploaded successfully.',
                    icon: 'success',
                    confirmButtonText: 'Ok',
                    allowOutsideClick: false
                })
            } catch (error) {
                Swal.fire({
                    title: 'Error!',
                    text: `UploadProfilePicture: ${error?.response?.data?.message || error}`,
                    icon: 'error',
                    confirmButtonText: 'Ok',
                    allowOutsideClick: false
                })
            }
        }

        const getDataTable = async () => {
            try {
                paged.tableLoading = true
                const response = await request('get', apiUrl.value, {})
                const data = response?.data?.content?.users
                if (data) {
                    paged.data = data
                    const response = await request('get', `/FileOperation/GetImage?imageName=${paged.data?.[0]?.profilePictureName || ''}`, {}, {}, 'blob')                    
                    imageUrl.value = URL.createObjectURL(response.data)
                }
            } catch (error) {
                Swal.fire({
                    title: 'Error!',
                    text: `getDataTable: ${error?.response?.data?.message || error}`,
                    icon: 'error',
                    confirmButtonText: 'Ok',
                    allowOutsideClick: false
                })
            } finally {
                paged.tableLoading = false
            }
        }

        const handleTableAction = async (payload) => {
            paged.limit = payload?.limit ?? paged.limit
            paged.page = payload?.page ?? paged.page
            paged.sortBy = payload?.sortBy ?? paged.sortBy
            paged.sortDirection = payload?.sortDirection ?? paged.sortDirection
            paged.selectedRow = null
            await getDataTable()
        }



        const handleTableActionRowClick = async (payload) => {
            changePageMode('read')
            paged.selectedRow = payload
            refreshClaimTable()
            refreshRoleTable()
        }



        const preparePage = async () => {
            await checkPageAccess()
            await getDataTable()
            const respClaimLookup = await request('get', '/Claim/GetClaimLookup', {})
            claimLookup.value = respClaimLookup?.data?.content?.data
            const respRoleLookup = await request('get', '/Role/GetRoleLookup', {})
            roleLookup.value = respRoleLookup?.data?.content?.data
            setTimeout(() => {
                pageLoading.value = false
            }, 100)
        }

        onMounted(async () => {
            await preparePage()
        })


        return {
            columns,
            pageLoading,
            successMessage,
            hasClaimAccess,
            getDataTable,
            handleTableAction,
            paged,
            formData,
            errorData,
            handleClickClose,
            handleClickEdit,
            handleClickDelete,
            handleSubmitForm,
            handleTableActionRowClick,
            userProfileClaims,
            userProfileRoles,
            claimsTypesSelected,
            rolesTypesSelected,
            pageMode,
            imageUrl,
            handleProfilePictureChange,
            uploadProfilePicture,
            selectedProfilePicture
        }
    }
})


app.config.errorHandler = (err, instance, info) => {
    Swal.fire({
        title: 'Error!',
        text: `Error: ${err}`,
        icon: 'error',
        confirmButtonText: 'Ok',
        allowOutsideClick: false,
        allowOutsideClick: false
    })
}
app.mount('#app')